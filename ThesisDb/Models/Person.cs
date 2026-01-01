using System;
using System.Collections.Generic;

namespace ThesisDb.Models;

public partial class Person
{
    public int PersonId { get; set; }

    public string Fname { get; set; } = null!;

    public string Lname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int InstituteId { get; set; }

    public virtual Institute Institute { get; set; } = null!;

    public virtual ICollection<ThesisPersonRole> ThesisPersonRoles { get; set; } = new List<ThesisPersonRole>();
}
