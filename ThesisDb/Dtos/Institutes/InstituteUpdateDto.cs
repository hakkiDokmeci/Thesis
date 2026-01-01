namespace ThesisDb.Dtos.Institutes;

public sealed class InstituteUpdateDto
{
    public string Name { get; set; } = default!;
    public int UniversityId { get; set; }
}
