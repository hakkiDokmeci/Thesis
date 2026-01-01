namespace ThesisDb.Dtos.Keywords;

public sealed class KeywordListDto
{
    public int KeywordId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

}
