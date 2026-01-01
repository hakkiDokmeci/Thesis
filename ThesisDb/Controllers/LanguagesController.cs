using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisDb.Data;
using ThesisDb.Dtos.Languages;
using ThesisDb.Models;

namespace ThesisDb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LanguagesController : ControllerBase
{
    private readonly GraduateThesisDbContext _db;

    public LanguagesController(GraduateThesisDbContext db)
    {
        _db = db;
    }

    // GET: /api/languages
    [HttpGet]
    public async Task<ActionResult<List<LanguageListDto>>> GetAll()
    {
        var list = await _db.Languages
            .AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => new LanguageListDto
            {
                LanguageId = l.LanguageId,
                Name = l.Name
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: /api/languages/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LanguageListDto>> GetById(int id)
    {
        var item = await _db.Languages
            .AsNoTracking()
            .Where(l => l.LanguageId == id)
            .Select(l => new LanguageListDto
            {
                LanguageId = l.LanguageId,
                Name = l.Name
            })
            .FirstOrDefaultAsync();

        if (item == null) return NotFound();
        return Ok(item);
    }

    // POST: /api/languages
    [HttpPost]
    public async Task<ActionResult<LanguageListDto>> Create([FromBody] LanguageCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        var name = dto.Name.Trim();

        var exists = await _db.Languages.AnyAsync(l => l.Name == name);
        if (exists) return Conflict("Language name already exists.");

        var entity = new Language { Name = name };

        _db.Languages.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.LanguageId }, new LanguageListDto
        {
            LanguageId = entity.LanguageId,
            Name = entity.Name
        });
    }

    // PUT: /api/languages/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<LanguageListDto>> Update(int id, [FromBody] LanguageUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        var entity = await _db.Languages.FindAsync(id);
        if (entity == null) return NotFound();

        var name = dto.Name.Trim();

        var taken = await _db.Languages.AnyAsync(l => l.LanguageId != id && l.Name == name);
        if (taken) return Conflict("Language name already exists.");

        entity.Name = name;
        await _db.SaveChangesAsync();

        return Ok(new LanguageListDto
        {
            LanguageId = entity.LanguageId,
            Name = entity.Name
        });
    }

    // DELETE: /api/languages/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Languages.FindAsync(id);
        if (entity == null) return NotFound();

        _db.Languages.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
