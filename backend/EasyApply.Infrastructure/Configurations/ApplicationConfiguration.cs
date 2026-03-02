using EasyApply.Core.Entites;
using EasyApply.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyApply.Infrastructure.Data.Configurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.ToTable("applications");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.JobId)
            .HasColumnName("job_id")
            .IsRequired();

        builder.Property(a => a.CandidateId)
            .HasColumnName("candidate_id")
            .IsRequired();

        builder.Property(a => a.CVId)
            .HasColumnName("cv_id")
            .IsRequired();

        builder.Property(a => a.CompatibilityScore)
            .HasColumnName("compatibility_score")
            .HasColumnType("decimal(5,2)");

        builder.Property(a => a.ScoreDetails)
            .HasColumnName("score_details")
            .HasColumnType("jsonb");

        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasDefaultValue(ApplicationStatus.Pending);

        builder.Property(a => a.AppliedAt)
            .HasColumnName("applied_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(a => a.ReviewedAt)
            .HasColumnName("reviewed_at");

        // Unique constraint
        builder.HasIndex(a => new { a.JobId, a.CandidateId })
            .IsUnique();

        // Indexes
        builder.HasIndex(a => a.JobId);
        builder.HasIndex(a => a.CandidateId);
        builder.HasIndex(a => a.CompatibilityScore)
            .IsDescending();
    }
}