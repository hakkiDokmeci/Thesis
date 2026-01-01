namespace ThesisDb.Dtos.Theses;

public sealed class ThesisDetailDto
{
    public int ThesisId { get; set; }
    public string Title { get; set; } = default!;
    public string Abstract { get; set; } = default!;
    public int Year { get; set; }
    public int NumberOfPages { get; set; }
    public DateTime? SubmissionDate { get; set; }

    public int TypeId { get; set; }
    public string TypeName { get; set; } = default!;

    public int LanguageId { get; set; }
    public string LanguageName { get; set; } = default!;

    public int InstituteId { get; set; }
    public string InstituteName { get; set; } = default!;
    public int UniversityId { get; set; }
    public string UniversityName { get; set; } = default!;

    public ThesisPersonDto? Author { get; set; }
    public List<ThesisPersonDto> Supervisors { get; set; } = new();
    public ThesisPersonDto? CoSupervisor { get; set; }

    public List<ThesisLookupItemDto> Subjects { get; set; } = new();
    public List<ThesisLookupItemDto> Keywords { get; set; } = new();
}
