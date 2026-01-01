using System;
using System.Collections.Generic;

namespace ThesisDb.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Thesis> Theses { get; set; } = new List<Thesis>();
}
