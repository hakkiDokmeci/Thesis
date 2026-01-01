using System;
using System.Collections.Generic;

namespace ThesisDb.Models;

public partial class ThesisType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Thesis> Theses { get; set; } = new List<Thesis>();
}
