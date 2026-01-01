using System;
using System.Collections.Generic;

namespace ThesisDb.Models;

public partial class Thesis
{
    public int ThesisId { get; set; }

    public string Title { get; set; } = null!;

    public string Abstract { get; set; } = null!;

    public int Year { get; set; }

    public int TypeId { get; set; }

    public int InstituteId { get; set; }

    public int NumberOfPages { get; set; }

    public int LanguageId { get; set; }

    public DateOnly SubmissionDate { get; set; }

    public virtual Institute Institute { get; set; } = null!;

    public virtual Language Language { get; set; } = null!;

    public virtual ICollection<ThesisPersonRole> ThesisPersonRoles { get; set; } = new List<ThesisPersonRole>();

    public virtual ThesisType Type { get; set; } = null!;

    public virtual ICollection<Keyword> Keywords { get; set; } = new List<Keyword>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
