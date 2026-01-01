using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ThesisDb.Models;

namespace ThesisDb.Data;

public partial class GraduateThesisDbContext : DbContext
{
    public GraduateThesisDbContext()
    {
    }

    public GraduateThesisDbContext(DbContextOptions<GraduateThesisDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Institute> Institutes { get; set; }

    public virtual DbSet<Keyword> Keywords { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Thesis> Theses { get; set; }

    public virtual DbSet<ThesisPersonRole> ThesisPersonRoles { get; set; }

    public virtual DbSet<ThesisType> ThesisTypes { get; set; }

    public virtual DbSet<University> Universities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configuration is now handled in Program.cs via dependency injection
        // If you need to run migrations or scaffold, use: dotnet ef with --connection parameter
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Institute>(entity =>
        {
            entity.HasKey(e => e.InstituteId).HasName("PK__INSTITUT__09EC0D9B3DCD74B4");

            entity.ToTable("INSTITUTE");

            entity.HasIndex(e => e.UniversityId, "IX_Institute_University");

            entity.HasIndex(e => new { e.Name, e.UniversityId }, "UQ_Institute_Name_University").IsUnique();

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UniversityId).HasColumnName("UniversityID");

            entity.HasOne(d => d.University).WithMany(p => p.Institutes)
                .HasForeignKey(d => d.UniversityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Institute_University");
        });

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.HasKey(e => e.KeywordId).HasName("PK__KEYWORD__37C135C1FB9507F9");

            entity.ToTable("KEYWORD");

            entity.HasIndex(e => e.Name, "UQ_Keyword_Name").IsUnique();

            entity.Property(e => e.KeywordId).HasColumnName("KeywordID");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PK__LANGUAGE__B938558BE84C05C1");

            entity.ToTable("LANGUAGE");

            entity.HasIndex(e => e.Name, "UQ_Language_Name").IsUnique();

            entity.Property(e => e.LanguageId).HasColumnName("LanguageID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__PERSON__AA2FFB85D4780D0B");

            entity.ToTable("PERSON");

            entity.HasIndex(e => e.InstituteId, "IX_Person_Institute");

            entity.HasIndex(e => e.Email, "UQ_Person_Email").IsUnique();

            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Fname)
                .HasMaxLength(100)
                .HasColumnName("FName");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Lname)
                .HasMaxLength(100)
                .HasColumnName("LName");

            entity.HasOne(d => d.Institute).WithMany(p => p.People)
                .HasForeignKey(d => d.InstituteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Person_Institute");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__SUBJECT__AC1BA388070DFBCE");

            entity.ToTable("SUBJECT");

            entity.HasIndex(e => e.Name, "UQ_Subject_Name").IsUnique();

            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Thesis>(entity =>
        {
            entity.HasKey(e => e.ThesisId).HasName("PK_Thesis");

            entity.ToTable("THESIS");

            entity.HasIndex(e => e.InstituteId, "IX_Thesis_Institute");

            entity.HasIndex(e => e.LanguageId, "IX_Thesis_Language");

            entity.HasIndex(e => e.TypeId, "IX_Thesis_Type");

            entity.Property(e => e.ThesisId)
                .ValueGeneratedNever()
                .HasColumnName("ThesisID");
            entity.Property(e => e.Abstract).HasMaxLength(4000);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.LanguageId).HasColumnName("LanguageID");
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.TypeId).HasColumnName("TypeID");

            entity.HasOne(d => d.Institute).WithMany(p => p.Theses)
                .HasForeignKey(d => d.InstituteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Thesis_Institute");

            entity.HasOne(d => d.Language).WithMany(p => p.Theses)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Thesis_Language");

            entity.HasOne(d => d.Type).WithMany(p => p.Theses)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Thesis_Type");

            entity.HasMany(d => d.Keywords).WithMany(p => p.Theses)
                .UsingEntity<Dictionary<string, object>>(
                    "ThesisKeyword",
                    r => r.HasOne<Keyword>().WithMany()
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ThesisKeyword_Keyword"),
                    l => l.HasOne<Thesis>().WithMany()
                        .HasForeignKey("ThesisId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ThesisKeyword_Thesis"),
                    j =>
                    {
                        j.HasKey("ThesisId", "KeywordId").HasName("PK_ThesisKeyword");
                        j.ToTable("THESIS_KEYWORD");
                        j.HasIndex(new[] { "KeywordId" }, "IX_ThesisKeyword_Keyword");
                        j.IndexerProperty<int>("ThesisId").HasColumnName("ThesisID");
                        j.IndexerProperty<int>("KeywordId").HasColumnName("KeywordID");
                    });

            entity.HasMany(d => d.Subjects).WithMany(p => p.Theses)
                .UsingEntity<Dictionary<string, object>>(
                    "ThesisSubject",
                    r => r.HasOne<Subject>().WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ThesisSubject_Subject"),
                    l => l.HasOne<Thesis>().WithMany()
                        .HasForeignKey("ThesisId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ThesisSubject_Thesis"),
                    j =>
                    {
                        j.HasKey("ThesisId", "SubjectId").HasName("PK_ThesisSubject");
                        j.ToTable("THESIS_SUBJECT");
                        j.HasIndex(new[] { "SubjectId" }, "IX_ThesisSubject_Subject");
                        j.IndexerProperty<int>("ThesisId").HasColumnName("ThesisID");
                        j.IndexerProperty<int>("SubjectId").HasColumnName("SubjectID");
                    });
        });

        modelBuilder.Entity<ThesisPersonRole>(entity =>
        {
            entity.HasKey(e => new { e.ThesisId, e.PersonId, e.RoleType }).HasName("PK_ThesisPersonRole");

            entity.ToTable("THESIS_PERSON_ROLE");

            entity.HasIndex(e => e.PersonId, "IX_ThesisPersonRole_Person");

            entity.Property(e => e.ThesisId).HasColumnName("ThesisID");
            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.RoleType).HasMaxLength(30);

            entity.HasOne(d => d.Person).WithMany(p => p.ThesisPersonRoles)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThesisPersonRole_Person");

            entity.HasOne(d => d.Thesis).WithMany(p => p.ThesisPersonRoles)
                .HasForeignKey(d => d.ThesisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThesisPersonRole_Thesis");
        });

        modelBuilder.Entity<ThesisType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__THESIS_T__516F039570E9A598");

            entity.ToTable("THESIS_TYPE");

            entity.HasIndex(e => e.TypeName, "UQ_ThesisType_Name").IsUnique();

            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<University>(entity =>
        {
            entity.HasKey(e => e.UniversityId).HasName("PK__UNIVERSI__9F19E19C3FC386FE");

            entity.ToTable("UNIVERSITY");

            entity.HasIndex(e => e.Name, "UQ_University_Name").IsUnique();

            entity.Property(e => e.UniversityId).HasColumnName("UniversityID");
            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
