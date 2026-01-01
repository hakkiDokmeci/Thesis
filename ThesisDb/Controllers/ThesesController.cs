using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisDb.Data;
using ThesisDb.Dtos.Theses;
using ThesisDb.Models;

namespace ThesisDb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThesesController : ControllerBase
{
    private readonly GraduateThesisDbContext _db;

    public ThesesController(GraduateThesisDbContext db)
    {
        _db = db;
    }

    // GET: /api/theses
    // GET: /api/theses?title=ai&year=2023&typeId=1&languageId=2&instituteId=3
    [HttpGet]
    public async Task<ActionResult<List<ThesisListDto>>> GetAll(
        [FromQuery] string? title,
        [FromQuery] int? year,
        [FromQuery] int? typeId,
        [FromQuery] int? languageId,
        [FromQuery] int? instituteId)
    {
        var q = _db.Theses
            .AsNoTracking()
            .Include(t => t.Type)
            .Include(t => t.Language)
            .Include(t => t.Institute)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            var term = title.Trim();
            q = q.Where(t => t.Title.Contains(term));
        }

        if (year.HasValue)
            q = q.Where(t => t.Year == year.Value);

        if (typeId.HasValue)
            q = q.Where(t => t.TypeId == typeId.Value);

        if (languageId.HasValue)
            q = q.Where(t => t.LanguageId == languageId.Value);

        if (instituteId.HasValue)
            q = q.Where(t => t.InstituteId == instituteId.Value);

        var list = await q
            .OrderByDescending(t => t.Year)
            .Select(t => new ThesisListDto
            {
                ThesisId = t.ThesisId,
                Title = t.Title,
                Year = t.Year,

                TypeId = t.TypeId,
                TypeName = t.Type.TypeName,

                LanguageId = t.LanguageId,
                LanguageName = t.Language.Name,

                InstituteId = t.InstituteId,
                InstituteName = t.Institute.Name
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ThesisCreateDto dto)
    {
        // 1) Basic validation
        if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title is required.");
        if (string.IsNullOrWhiteSpace(dto.Abstract)) return BadRequest("Abstract is required.");
        if (dto.Year <= 1900 || dto.Year > DateTime.UtcNow.Year + 1) return BadRequest("Year is invalid.");
        if (dto.NumberOfPages <= 0) return BadRequest("NumberOfPages must be > 0.");

        if (dto.TypeId <= 0) return BadRequest("TypeId is required.");
        if (dto.LanguageId <= 0) return BadRequest("LanguageId is required.");
        if (dto.InstituteId <= 0) return BadRequest("InstituteId is required.");

        if (dto.AuthorId <= 0) return BadRequest("AuthorId is required.");
        if (dto.SupervisorIds == null || dto.SupervisorIds.Count == 0) return BadRequest("At least 1 supervisor is required.");
        if (dto.SubjectIds == null || dto.SubjectIds.Count == 0) return BadRequest("At least 1 subject is required.");

        dto.SupervisorIds = dto.SupervisorIds.Distinct().ToList();
        dto.SubjectIds = dto.SubjectIds.Distinct().ToList();

        // 2) FK checks
        if (!await _db.ThesisTypes.AnyAsync(x => x.TypeId == dto.TypeId))
            return BadRequest("Type does not exist.");

        if (!await _db.Languages.AnyAsync(x => x.LanguageId == dto.LanguageId))
            return BadRequest("Language does not exist.");

        if (!await _db.Institutes.AnyAsync(x => x.InstituteId == dto.InstituteId))
            return BadRequest("Institute does not exist.");

        if (!await _db.People.AnyAsync(p => p.PersonId == dto.AuthorId))
            return BadRequest("Author does not exist.");

        var supervisorsCount = await _db.People.CountAsync(p => dto.SupervisorIds.Contains(p.PersonId));
        if (supervisorsCount != dto.SupervisorIds.Count)
            return BadRequest("One or more supervisors do not exist.");

        if (dto.CoSupervisorId.HasValue)
        {
            if (!await _db.People.AnyAsync(p => p.PersonId == dto.CoSupervisorId.Value))
                return BadRequest("CoSupervisor does not exist.");
        }

        var subjectsCount = await _db.Subjects.CountAsync(s => dto.SubjectIds.Contains(s.SubjectId));
        if (subjectsCount != dto.SubjectIds.Count)
            return BadRequest("One or more subjects do not exist.");

        // 3) Keywords normalize
        var cleanedKeywords = (dto.Keywords ?? new List<string>())
            .Select(k => k?.Trim())
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(k => k!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // 4) Transaction
        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            // 4.1 Insert THESIS
            var thesis = new Thesis
            {
                Title = dto.Title.Trim(),
                Abstract = dto.Abstract.Trim(),
                Year = dto.Year,
                NumberOfPages = dto.NumberOfPages,
                SubmissionDate = dto.SubmissionDate.HasValue
                    ? DateOnly.FromDateTime(dto.SubmissionDate.Value)
                    : DateOnly.FromDateTime(DateTime.UtcNow),

                TypeId = dto.TypeId,
                LanguageId = dto.LanguageId,
                InstituteId = dto.InstituteId
            };

            // ThesisID identity değil -> biz üreteceğiz (1..9999999)
            var nextId = (await _db.Theses.MaxAsync(t => (int?)t.ThesisId) ?? 0) + 1;

            if (nextId < 1 || nextId > 9999999)
                return BadRequest("ThesisID range exceeded (1..9999999).");

            thesis.ThesisId = nextId;


            _db.Theses.Add(thesis);
            await _db.SaveChangesAsync(); // ThesisId oluşsun

            // 4.2 Roles (THESIS_PERSON_ROLE)
            _db.ThesisPersonRoles.Add(new ThesisPersonRole
            {
                ThesisId = thesis.ThesisId,
                PersonId = dto.AuthorId,
                RoleType = "AUTHOR"
            });

            foreach (var supId in dto.SupervisorIds)
            {
                _db.ThesisPersonRoles.Add(new ThesisPersonRole
                {
                    ThesisId = thesis.ThesisId,
                    PersonId = supId,
                    RoleType = "SUPERVISOR"
                });
            }

            if (dto.CoSupervisorId.HasValue)
            {
                _db.ThesisPersonRoles.Add(new ThesisPersonRole
                {
                    ThesisId = thesis.ThesisId,
                    PersonId = dto.CoSupervisorId.Value,
                    RoleType = "CO_SUPERVISOR"
                });
            }

            // 4.3 Subjects (skip navigation) -> THESIS_SUBJECT
            // Thesis modelinde `public virtual ICollection<Subject> Subjects {get;set;}` olmalı
            var subjectEntities = await _db.Subjects
                .Where(s => dto.SubjectIds.Contains(s.SubjectId))
                .ToListAsync();

            foreach (var s in subjectEntities)
                thesis.Subjects.Add(s);

            // 4.4 Keywords upsert + skip navigation -> THESIS_KEYWORD
            // Thesis modelinde `public virtual ICollection<Keyword> Keywords {get;set;}` olmalı
            foreach (var kw in cleanedKeywords)
            {
                var keywordEntity = await _db.Keywords.FirstOrDefaultAsync(k => k.Name == kw);

                if (keywordEntity == null)
                {
                    keywordEntity = new Keyword
                    {
                        Name = kw
                        // Description varsa burada doldurabilirsin
                    };

                    _db.Keywords.Add(keywordEntity);
                    await _db.SaveChangesAsync(); // KeywordId oluşsun
                }

                thesis.Keywords.Add(keywordEntity);
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return CreatedAtAction(nameof(GetById), new { id = thesis.ThesisId }, new { thesisId = thesis.ThesisId });
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ThesisDetailDto>> GetById(int id)
    {
        var t = await _db.Theses
            .AsNoTracking()
            .Include(x => x.Type)
            .Include(x => x.Language)
            .Include(x => x.Institute)
                .ThenInclude(i => i.University)
            .Include(x => x.Subjects)  // ✅ THESIS_SUBJECT üzerinden gelir
            .Include(x => x.Keywords)  // ✅ THESIS_KEYWORD üzerinden gelir
            .FirstOrDefaultAsync(x => x.ThesisId == id);

        if (t == null)
            return NotFound();

        // Roles + Person
        var roles = await _db.ThesisPersonRoles
            .AsNoTracking()
            .Where(r => r.ThesisId == id)
            .Include(r => r.Person)
            .ToListAsync();

        var author = roles.FirstOrDefault(r => r.RoleType == "AUTHOR")?.Person;
        var supervisors = roles.Where(r => r.RoleType == "SUPERVISOR").Select(r => r.Person).Where(p => p != null).ToList();
        var coSup = roles.FirstOrDefault(r => r.RoleType == "CO_SUPERVISOR")?.Person;

        ThesisPersonDto MapPerson(Person p) => new ThesisPersonDto
        {
            PersonId = p.PersonId,
            FName = p.Fname,
            LName = p.Lname,
            Email = p.Email
        };

        var dto = new ThesisDetailDto
        {
            ThesisId = t.ThesisId,
            Title = t.Title,
            Abstract = t.Abstract,
            Year = t.Year,
            NumberOfPages = t.NumberOfPages,

            SubmissionDate = new DateTime(t.SubmissionDate.Year, t.SubmissionDate.Month, t.SubmissionDate.Day),

            TypeId = t.TypeId,
            TypeName = t.Type.TypeName,

            LanguageId = t.LanguageId,
            LanguageName = t.Language.Name,

            InstituteId = t.InstituteId,
            InstituteName = t.Institute.Name,

            UniversityId = t.Institute.UniversityId,
            UniversityName = t.Institute.University.Name,

            Author = author == null ? null : MapPerson(author),
            Supervisors = supervisors.Select(MapPerson).ToList(),
            CoSupervisor = coSup == null ? null : MapPerson(coSup),

            Subjects = t.Subjects
                .Select(s => new ThesisLookupItemDto { Id = s.SubjectId, Name = s.Name })
                .ToList(),

            Keywords = t.Keywords
                .Select(k => new ThesisLookupItemDto { Id = k.KeywordId, Name = k.Name })
                .ToList()
        };

        return Ok(dto);
    }

    // GET: /api/theses/search
    // Example:
    // /api/theses/search?title=ai&yearMin=2020&yearMax=2024&subjectId=3&keyword=nlp&author=ahmet
    [HttpGet("search")]
    public async Task<ActionResult<List<ThesisListDto>>> Search(
        [FromQuery] string? title,
        [FromQuery] int? yearMin,
        [FromQuery] int? yearMax,
        [FromQuery] int? typeId,
        [FromQuery] int? languageId,
        [FromQuery] int? instituteId,
        [FromQuery] int? subjectId,
        [FromQuery] string? keyword,
        [FromQuery] string? author)
    {
        var q = _db.Theses
            .AsNoTracking()
            .Include(t => t.Type)
            .Include(t => t.Language)
            .Include(t => t.Institute)
            .Include(t => t.Subjects)   // M:N
            .Include(t => t.Keywords)   // M:N
            .AsQueryable();

        // ---------- BASIC FILTERS ----------
        if (!string.IsNullOrWhiteSpace(title))
            q = q.Where(t => t.Title.Contains(title));

        if (yearMin.HasValue)
            q = q.Where(t => t.Year >= yearMin.Value);

        if (yearMax.HasValue)
            q = q.Where(t => t.Year <= yearMax.Value);

        if (typeId.HasValue)
            q = q.Where(t => t.TypeId == typeId.Value);

        if (languageId.HasValue)
            q = q.Where(t => t.LanguageId == languageId.Value);

        if (instituteId.HasValue)
            q = q.Where(t => t.InstituteId == instituteId.Value);

        // ---------- SUBJECT (M:N) ----------
        if (subjectId.HasValue)
            q = q.Where(t => t.Subjects.Any(s => s.SubjectId == subjectId.Value));

        // ---------- KEYWORD (M:N, name based) ----------
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            q = q.Where(t => t.Keywords.Any(k => k.Name.Contains(kw)));
        }

        // ---------- AUTHOR / SUPERVISOR ----------
        if (!string.IsNullOrWhiteSpace(author))
        {
            var term = author.Trim();

            q = q.Where(t =>
                _db.ThesisPersonRoles.Any(r =>
                    r.ThesisId == t.ThesisId &&
                    (r.RoleType == "AUTHOR" || r.RoleType == "SUPERVISOR") &&
                    (r.Person.Fname.Contains(term) || r.Person.Lname.Contains(term))
                )
            );
        }

        // ---------- RESULT ----------
        var list = await q
            .OrderByDescending(t => t.Year)
            .Select(t => new ThesisListDto
            {
                ThesisId = t.ThesisId,
                Title = t.Title,
                Year = t.Year,

                TypeId = t.TypeId,
                TypeName = t.Type.TypeName,

                LanguageId = t.LanguageId,
                LanguageName = t.Language.Name,

                InstituteId = t.InstituteId,
                InstituteName = t.Institute.Name
            })
            .ToListAsync();

        return Ok(list);
    }



}




