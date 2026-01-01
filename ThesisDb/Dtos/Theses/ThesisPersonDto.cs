namespace ThesisDb.Dtos.Theses;

public sealed class ThesisPersonDto
{
    public int PersonId { get; set; }
    public string FName { get; set; } = default!;
    public string LName { get; set; } = default!;
    public string Email { get; set; } = default!;
}
