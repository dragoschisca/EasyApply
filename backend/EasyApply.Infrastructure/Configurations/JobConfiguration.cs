using EasyApply.Domains.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyApply.Infrastructure.Data.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("jobs");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(j => j.CompanyId)
            .HasColumnName("company_id")
            .IsRequired();

        builder.Property(j => j.Title)
            .HasColumnName("title")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(j => j.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(j => j.Requirements)
            .HasColumnName("requirements")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(j => j.RequiredSkills)
            .HasColumnName("required_skills")
            .HasColumnType("jsonb");

        builder.Property(j => j.EmploymentType)
            .HasColumnName("employment_type")
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(j => j.ExperienceLevel)
            .HasColumnName("experience_level")
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(j => j.Location)
            .HasColumnName("location")
            .HasMaxLength(255);

        builder.Property(j => j.SalaryMin)
            .HasColumnName("salary_min")
            .HasColumnType("decimal(10,2)");

        builder.Property(j => j.SalaryMax)
            .HasColumnName("salary_max")
            .HasColumnType("decimal(10,2)");

        builder.Property(j => j.IsRemote)
            .HasColumnName("is_remote")
            .HasDefaultValue(false);

        builder.Property(j => j.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(j => j.ViewsCount)
            .HasColumnName("views_count")
            .HasDefaultValue(0);

        builder.Property(j => j.ApplicationsCount)
            .HasColumnName("applications_count")
            .HasDefaultValue(0);

        builder.Property(j => j.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(j => j.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(j => j.ExpiresAt)
            .HasColumnName("expires_at");

        // Indexes
        builder.HasIndex(j => j.CompanyId);
        builder.HasIndex(j => j.IsActive);
        builder.HasIndex(j => j.CreatedAt);

        // Relationships
        builder.HasMany(j => j.Applications)
            .WithOne(a => a.Job)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(j => j.SavedJobs)
            .WithOne(sj => sj.Job)
            .HasForeignKey(sj => sj.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}