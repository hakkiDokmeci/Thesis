using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisDb.Data;
using ThesisDb.Dtos.Subjects;
using ThesisDb.Models;

namespace ThesisDb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly GraduateThesisDbContext _db;

    public SubjectsController(GraduateThesisDbContext db)
    {
        _db = db;
    }

    // GET: /api/subjects
    [HttpGet]
    public async Task<ActionResult<List<SubjectListDto>>> GetAll()
    {
        var list = await _db.Subjects
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SubjectListDto
            {
                SubjectId = s.SubjectId,
                Name = s.Name
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: /api/subjects/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectListDto>> GetById(int id)
    {
        var item = await _db.Subjects
            .AsNoTracking()
            .Where(s => s.SubjectId == id)
            .Select(s => new SubjectListDto
            {
                SubjectId = s.SubjectId,
                Name = s.Name
            })
            .FirstOrDefaultAsync();

        if (item == null) return NotFound();
        return Ok(item);
    }

    // POST: /api/subjects
    [HttpPost]
    public async Task<ActionResult<SubjectListDto>> Create([FromBody] SubjectCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        var name = dto.Name.Trim();

        var exists = await _db.Subjects.AnyAsync(s => s.Name == name);
        if (exists)
            return Conflict("Subject name already exists.");

        var entity = new Subject
        {
            Name = name
        };

        _db.Subjects.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.SubjectId }, new SubjectListDto
        {
            SubjectId = entity.SubjectId,
            Name = entity.Name
        });
    }

    // PUT: /api/subjects/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<SubjectListDto>> Update(int id, [FromBody] SubjectUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        var entity = await _db.Subjects.FindAsync(id);
        if (entity == null) return NotFound();

        var name = dto.Name.Trim();

        var taken = await _db.Subjects.AnyAsync(s => s.SubjectId != id && s.Name == name);
        if (taken)
            return Conflict("Subject name already exists.");

        entity.Name = name;
        await _db.SaveChangesAsync();

        return Ok(new SubjectListDto
        {
            SubjectId = entity.SubjectId,
            Name = entity.Name
        });
    }

    // DELETE: /api/subjects/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Subjects.FindAsync(id);
        if (entity == null) return NotFound();

        _db.Subjects.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
