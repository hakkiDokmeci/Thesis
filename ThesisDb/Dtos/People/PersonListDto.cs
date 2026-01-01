namespace ThesisDb.Dtos.People;

public sealed class PersonListDto
{
    public int PersonId { get; set; }
    public string FName { get; set; } = default!;
    public string LName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public int? InstituteId { get; set; }
}
