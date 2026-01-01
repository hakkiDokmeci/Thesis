namespace ThesisDb.Dtos.Subjects;

public sealed class SubjectListDto
{
    public int SubjectId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    
}
