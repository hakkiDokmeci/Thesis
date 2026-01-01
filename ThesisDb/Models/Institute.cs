using System;
using System.Collections.Generic;

namespace ThesisDb.Models;

public partial class Institute
{
    public int InstituteId { get; set; }

    public string Name { get; set; } = null!;

    public int UniversityId { get; set; }

    public virtual ICollection<Person> People { get; set; } = new List<Person>();

    public virtual ICollection<Thesis> Theses { get; set; } = new List<Thesis>();

    public virtual University University { get; set; } = null!;
}
