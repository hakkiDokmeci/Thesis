using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisDb.Data;               // DbContext namespace (sende neyse onu yaz)
using ThesisDb.Dtos.Institutes;    // DTO namespace
using ThesisDb.Models;             // Institute entity namespace

namespace ThesisDb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstitutesController : ControllerBase
{
    private readonly GraduateThesisDbContext _db;

    public InstitutesController(GraduateThesisDbContext db)
    {
        _db = db;
    }

    // GET: /api/institutes
    // GET: /api/institutes?universityId=1
    [HttpGet]
    public async Task<ActionResult<List<InstituteListDto>>> GetAll([FromQuery] int? universityId)
    {
        var q = _db.Institutes
            .AsNoTracking()
            .Include(i => i.University)  // navigation varsa
            .AsQueryable();

        if (universityId.HasValue && universityId.Value > 0)
            q = q.Where(i => i.UniversityId == universityId.Value);

        var list = await q
            .OrderBy(i => i.Name)
            .Select(i => new InstituteListDto
            {
                InstituteId = i.InstituteId,
                Name = i.Name,
                UniversityId = i.UniversityId,
                UniversityName = i.University != null ? i.University.Name : null
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: /api/institutes/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<InstituteListDto>> GetById(int id)
    {
        var item = await _db.Institutes
            .AsNoTracking()
            .Include(i => i.University)
            .Where(i => i.InstituteId == id)
            .Select(i => new InstituteListDto
            {
                InstituteId = i.InstituteId,
                Name = i.Name,
                UniversityId = i.UniversityId,
                UniversityName = i.University != null ? i.University.Name : null
            })
            .FirstOrDefaultAsync();

        if (item == null) return NotFound();
        return Ok(item);
    }

    // POST: /api/institutes
    [HttpPost]
    public async Task<ActionResult<InstituteListDto>> Create([FromBody] InstituteCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        if (dto.UniversityId <= 0)
            return BadRequest("UniversityId is required.");

        // FK check
        var uniExists = await _db.Universities.AnyAsync(u => u.UniversityId == dto.UniversityId);
        if (!uniExists)
            return BadRequest("University does not exist.");

        // duplicate check (same name under same university)
        var name = dto.Name.Trim();
        var dup = await _db.Institutes.AnyAsync(i => i.UniversityId == dto.UniversityId && i.Name == name);
        if (dup)
            return Conflict("Institute already exists for this university.");

        var entity = new Institute
        {
            Name = name,
            UniversityId = dto.UniversityId
        };

        _db.Institutes.Add(entity);
        await _db.SaveChangesAsync();

        // University name döndürmek için tekrar çekiyoruz
        var uniName = await _db.Universities
            .Where(u => u.UniversityId == entity.UniversityId)
            .Select(u => u.Name)
            .FirstAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.InstituteId }, new InstituteListDto
        {
            InstituteId = entity.InstituteId,
            Name = entity.Name,
            UniversityId = entity.UniversityId,
            UniversityName = uniName
        });
    }

    // PUT: /api/institutes/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<InstituteListDto>> Update(int id, [FromBody] InstituteUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        if (dto.UniversityId <= 0)
            return BadRequest("UniversityId is required.");

        var entity = await _db.Institutes.FindAsync(id);
        if (entity == null) return NotFound();

        // FK check
        var uniExists = await _db.Universities.AnyAsync(u => u.UniversityId == dto.UniversityId);
        if (!uniExists)
            return BadRequest("University does not exist.");

        var name = dto.Name.Trim();

        // duplicate check (same name under same university, excluding current)
        var dup = await _db.Institutes.AnyAsync(i =>
            i.InstituteId != id &&
            i.UniversityId == dto.UniversityId &&
            i.Name == name);

        if (dup)
            return Conflict("Institute already exists for this university.");

        entity.Name = name;
        entity.UniversityId = dto.UniversityId;

        await _db.SaveChangesAsync();

        var uniName = await _db.Universities
            .Where(u => u.UniversityId == entity.UniversityId)
            .Select(u => u.Name)
            .FirstAsync();

        return Ok(new InstituteListDto
        {
            InstituteId = entity.InstituteId,
            Name = entity.Name,
            UniversityId = entity.UniversityId,
            UniversityName = uniName
        });
    }

    // DELETE: /api/institutes/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Institutes.FindAsync(id);
        if (entity == null) return NotFound();

        _db.Institutes.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
