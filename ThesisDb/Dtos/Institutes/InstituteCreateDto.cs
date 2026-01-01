namespace ThesisDb.Dtos.Institutes;

public sealed class InstituteCreateDto
{
    public string Name { get; set; } = default!;
    public int UniversityId { get; set; }
}
