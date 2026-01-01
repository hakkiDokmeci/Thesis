using System;
using System.Collections.Generic;

namespace ThesisDb.Models;

public partial class University
{
    public int UniversityId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<Institute> Institutes { get; set; } = new List<Institute>();
}
