using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisDb.Data;
using ThesisDb.Dtos.Keywords;
using ThesisDb.Models;

namespace ThesisDb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KeywordsController : ControllerBase
{
    private readonly GraduateThesisDbContext _db;

    public KeywordsController(GraduateThesisDbContext db)
    {
        _db = db;
    }

    // GET: /api/keywords
    [HttpGet]
    public async Task<ActionResult<List<KeywordListDto>>> GetAll()
    {
        var list = await _db.Keywords
            .AsNoTracking()
            .OrderBy(k => k.Name)
            .Select(k => new KeywordListDto
            {
                KeywordId = k.KeywordId,
                Name = k.Name
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: /api/keywords/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<KeywordListDto>> GetById(int id)
    {
        var item = await _db.Keywords
            .AsNoTracking()
            .Where(k => k.KeywordId == id)
            .Select(k => new KeywordListDto
            {
                KeywordId = k.KeywordId,
                Name = k.Name
            })
            .FirstOrDefaultAsync();

        if (item == null) return NotFound();
        return Ok(item);
    }

    // POST: /api/keywords
    [HttpPost]
    public async Task<ActionResult<KeywordListDto>> Create([FromBody] KeywordCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        var name = dto.Name.Trim();

        var exists = await _db.Keywords.AnyAsync(k => k.Name == name);
        if (exists)
            return Conflict("Keyword name already exists.");

        var entity = new Keyword
        {
            Name = name
        };

        _db.Keywords.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.KeywordId }, new KeywordListDto
        {
            KeywordId = entity.KeywordId,
            Name = entity.Name
        });
    }

    // PUT: /api/keywords/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<KeywordListDto>> Update(int id, [FromBody] KeywordUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        var entity = await _db.Keywords.FindAsync(id);
        if (entity == null) return NotFound();

        var name = dto.Name.Trim();

        var taken = await _db.Keywords.AnyAsync(k => k.KeywordId != id && k.Name == name);
        if (taken)
            return Conflict("Keyword name already exists.");

        entity.Name = name;
        await _db.SaveChangesAsync();

        return Ok(new KeywordListDto
        {
            KeywordId = entity.KeywordId,
            Name = entity.Name
        });
    }

    // DELETE: /api/keywords/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Keywords.FindAsync(id);
        if (entity == null) return NotFound();

        _db.Keywords.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
