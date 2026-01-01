using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisDb.Data;
using ThesisDb.Dtos.People;
using ThesisDb.Models;

namespace ThesisDb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    private readonly GraduateThesisDbContext _db;

    public PeopleController(GraduateThesisDbContext db)
    {
        _db = db;
    }

    // GET: /api/people?search=ahmet
    [HttpGet]
    public async Task<ActionResult<List<PersonListDto>>> GetAll([FromQuery] string? search)
    {
        var q = _db.People.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            q = q.Where(p =>
                p.Fname.Contains(term) ||
                p.Lname.Contains(term) ||
                p.Email.Contains(term));
        }

        var list = await q
            .OrderBy(p => p.Lname).ThenBy(p => p.Fname)
            .Select(p => new PersonListDto
            {
                PersonId = p.PersonId,
                FName = p.Fname,
                LName = p.Lname,
                Email = p.Email,
                InstituteId = p.InstituteId
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: /api/people/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PersonListDto>> GetById(int id)
    {
        var item = await _db.People
            .AsNoTracking()
            .Where(p => p.PersonId == id)
            .Select(p => new PersonListDto
            {
                PersonId = p.PersonId,
                FName = p.Fname,
                LName = p.Lname,
                Email = p.Email,
                InstituteId = p.InstituteId
            })
            .FirstOrDefaultAsync();

        if (item == null) return NotFound();
        return Ok(item);
    }

    // POST: /api/people
    [HttpPost]
    public async Task<ActionResult<PersonListDto>> Create([FromBody] PersonCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FName)) return BadRequest("FName is required.");
        if (string.IsNullOrWhiteSpace(dto.LName)) return BadRequest("LName is required.");
        if (string.IsNullOrWhiteSpace(dto.Email)) return BadRequest("Email is required.");

        var email = dto.Email.Trim();

        var emailTaken = await _db.People.AnyAsync(p => p.Email == email);
        if (emailTaken) return Conflict("Email already exists.");

        // Institute FK check (InstituteId null olabilir)
        if (dto.InstituteId.HasValue)
        {
            var instExists = await _db.Institutes.AnyAsync(i => i.InstituteId == dto.InstituteId.Value);
            if (!instExists) return BadRequest("Institute does not exist.");
        }

        var entity = new Person
        {
            Fname = dto.FName.Trim(),
            Lname = dto.LName.Trim(),
            Email = email,
            InstituteId = dto.InstituteId ?? 0 // Fixes CS0266 and CS8629
        };

        _db.People.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.PersonId }, new PersonListDto
        {
            PersonId = entity.PersonId,
            FName = entity.Fname,
            LName = entity.Lname,
            Email = entity.Email,
            InstituteId = entity.InstituteId
        });
    }

    // PUT: /api/people/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<PersonListDto>> Update(int id, [FromBody] PersonUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FName)) return BadRequest("FName is required.");
        if (string.IsNullOrWhiteSpace(dto.LName)) return BadRequest("LName is required.");
        if (string.IsNullOrWhiteSpace(dto.Email)) return BadRequest("Email is required.");

        var entity = await _db.People.FindAsync(id);
        if (entity == null) return NotFound();

        var email = dto.Email.Trim();

        var emailTaken = await _db.People.AnyAsync(p => p.PersonId != id && p.Email == email);
        if (emailTaken) return Conflict("Email already exists.");

        if (dto.InstituteId.HasValue)
        {
            var instExists = await _db.Institutes.AnyAsync(i => i.InstituteId == dto.InstituteId.Value);
            if (!instExists) return BadRequest("Institute does not exist.");
        }

        entity.Fname = dto.FName.Trim();
        entity.Lname = dto.LName.Trim();
        entity.Email = email;
        entity.InstituteId = dto.InstituteId ?? 0; // Fixes CS0266 and CS8629

        await _db.SaveChangesAsync();

        return Ok(new PersonListDto
        {
            PersonId = entity.PersonId,
            FName = entity.Fname,
            LName = entity.Lname,
            Email = entity.Email,
            InstituteId = entity.InstituteId
        });
    }

    // DELETE: /api/people/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.People.FindAsync(id);
        if (entity == null) return NotFound();

        _db.People.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
