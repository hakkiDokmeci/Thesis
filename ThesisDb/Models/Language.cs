using System;
using System.Collections.Generic;

namespace ThesisDb.Models;

public partial class Language
{
    public int LanguageId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Thesis> Theses { get; set; } = new List<Thesis>();
}
