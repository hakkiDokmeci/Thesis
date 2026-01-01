namespace ThesisDb.Dtos.Theses;

public sealed class ThesisListDto
{
    public int ThesisId { get; set; }
    public string Title { get; set; } = default!;
    public int Year { get; set; }

    public int TypeId { get; set; }
    public string TypeName { get; set; } = default!;

    public int LanguageId { get; set; }
    public string LanguageName { get; set; } = default!;

    public int InstituteId { get; set; }
    public string InstituteName { get; set; } = default!;
}
