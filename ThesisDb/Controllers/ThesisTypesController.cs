using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisDb.Data;
using ThesisDb.Dtos.ThesisTypes;
using ThesisDb.Models;

namespace ThesisDb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThesisTypesController : ControllerBase
{
    private readonly GraduateThesisDbContext _db;

    public ThesisTypesController(GraduateThesisDbContext db)
    {
        _db = db;
    }

    // GET: /api/thesistypes
    [HttpGet]
    public async Task<ActionResult<List<ThesisTypeListDto>>> GetAll()
    {
        var list = await _db.ThesisTypes
            .AsNoTracking()
            .OrderBy(t => t.TypeName)
            .Select(t => new ThesisTypeListDto
            {
                TypeId = t.TypeId,
                TypeName = t.TypeName
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: /api/thesistypes/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ThesisTypeListDto>> GetById(int id)
    {
        var item = await _db.ThesisTypes
            .AsNoTracking()
            .Where(t => t.TypeId == id)
            .Select(t => new ThesisTypeListDto
            {
                TypeId = t.TypeId,
                TypeName = t.TypeName
            })
            .FirstOrDefaultAsync();

        if (item == null) return NotFound();
        return Ok(item);
    }

    // POST: /api/thesistypes
    [HttpPost]
    public async Task<ActionResult<ThesisTypeListDto>> Create([FromBody] ThesisTypeCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TypeName))
            return BadRequest("TypeName is required.");

        var name = dto.TypeName.Trim();

        var exists = await _db.ThesisTypes.AnyAsync(t => t.TypeName == name);
        if (exists) return Conflict("Thesis type already exists.");

        var entity = new ThesisType
        {
            TypeName = name
        };

        _db.ThesisTypes.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.TypeId }, new ThesisTypeListDto
        {
            TypeId = entity.TypeId,
            TypeName = entity.TypeName
        });
    }

    // PUT: /api/thesistypes/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ThesisTypeListDto>> Update(int id, [FromBody] ThesisTypeUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TypeName))
            return BadRequest("TypeName is required.");

        var entity = await _db.ThesisTypes.FindAsync(id);
        if (entity == null) return NotFound();

        var name = dto.TypeName.Trim();

        var taken = await _db.ThesisTypes.AnyAsync(t => t.TypeId != id && t.TypeName == name);
        if (taken) return Conflict("Thesis type already exists.");

        entity.TypeName = name;
        await _db.SaveChangesAsync();

        return Ok(new ThesisTypeListDto
        {
            TypeId = entity.TypeId,
            TypeName = entity.TypeName
        });
    }

    // DELETE: /api/thesistypes/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.ThesisTypes.FindAsync(id);
        if (entity == null) return NotFound();

        _db.ThesisTypes.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
