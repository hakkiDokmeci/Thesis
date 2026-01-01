using Dtos.Universities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisDb.Data;
using ThesisDb.Models;

namespace YourNamespace.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UniversitiesController : ControllerBase
{
    private readonly GraduateThesisDbContext _db;

    public UniversitiesController(GraduateThesisDbContext db)
    {
        _db = db;
    }

    // GET: /api/universities
    [HttpGet]
    public async Task<ActionResult<List<UniversityListDto>>> GetAll()
    {
        var list = await _db.Universities
            .AsNoTracking()
            .OrderBy(u => u.Name)
            .Select(u => new UniversityListDto
            {
                UniversityId = u.UniversityId,
                Name = u.Name
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: /api/universities/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UniversityListDto>> GetById(int id)
    {
        var item = await _db.Universities
            .AsNoTracking()
            .Where(u => u.UniversityId == id)
            .Select(u => new UniversityListDto
            {
                UniversityId = u.UniversityId,
                Name = u.Name
            })
            .FirstOrDefaultAsync();

        if (item == null) return NotFound();
        return Ok(item);
    }

    // POST: /api/universities
    [HttpPost]
    public async Task<ActionResult<UniversityListDto>> Create([FromBody] UniversityCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        var exists = await _db.Universities.AnyAsync(u => u.Name == dto.Name);
        if (exists)
            return Conflict("University name already exists.");

        var entity = new University
        {
            Name = dto.Name.Trim()
        };

        _db.Universities.Add(entity);
        await _db.SaveChangesAsync();

        var result = new UniversityListDto
        {
            UniversityId = entity.UniversityId,
            Name = entity.Name
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.UniversityId }, result);
    }

    // PUT: /api/universities/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UniversityListDto>> Update(int id, [FromBody] UniversityUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        var entity = await _db.Universities.FindAsync(id);
        if (entity == null) return NotFound();

        var newName = dto.Name.Trim();
        var nameTaken = await _db.Universities.AnyAsync(u => u.UniversityId != id && u.Name == newName);
        if (nameTaken) return Conflict("University name already exists.");

        entity.Name = newName;
        await _db.SaveChangesAsync();

        return Ok(new UniversityListDto
        {
            UniversityId = entity.UniversityId,
            Name = entity.Name
        });
    }

    // DELETE: /api/universities/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Universities.FindAsync(id);
        if (entity == null) return NotFound();

        _db.Universities.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
