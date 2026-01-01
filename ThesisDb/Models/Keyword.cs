using System;
using System.Collections.Generic;

namespace ThesisDb.Models;

public partial class Keyword
{
    public int KeywordId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Thesis> Theses { get; set; } = new List<Thesis>();
}
