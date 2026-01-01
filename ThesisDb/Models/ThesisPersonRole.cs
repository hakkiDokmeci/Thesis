using System;
using System.Collections.Generic;

namespace ThesisDb.Models;

public partial class ThesisPersonRole
{
    public int ThesisId { get; set; }

    public int PersonId { get; set; }

    public string RoleType { get; set; } = null!;

    public virtual Person Person { get; set; } = null!;

    public virtual Thesis Thesis { get; set; } = null!;
}
