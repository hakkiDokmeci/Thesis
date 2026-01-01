namespace ThesisDb.Dtos.Institutes;

public sealed class InstituteListDto
{
    public int InstituteId { get; set; }
    public string Name { get; set; } = default!;
    public int UniversityId { get; set; }
    public string? UniversityName { get; set; } // UI'da göstermek için
}
