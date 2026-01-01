namespace ThesisDb.Dtos.Theses;

public sealed class ThesisCreateDto
{
    // THESIS
    public string Title { get; set; } = default!;
    public string Abstract { get; set; } = default!;
    public int Year { get; set; }
    public int NumberOfPages { get; set; }
    public DateTime? SubmissionDate { get; set; }

    public int TypeId { get; set; }
    public int LanguageId { get; set; }
    public int InstituteId { get; set; }

    // Roles
    public int AuthorId { get; set; }
    public List<int> SupervisorIds { get; set; } = new();
    public int? CoSupervisorId { get; set; }

    // Subjects & Keywords
    public List<int> SubjectIds { get; set; } = new();

    // UI keyword textbox’larından gelen string’ler
    public List<string> Keywords { get; set; } = new();
}
