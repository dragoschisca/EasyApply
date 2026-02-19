using EasyApply.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyApply.Infrastructure.Data.Configurations;

public class CVConfiguration : IEntityTypeConfiguration<CV>
{
    public void Configure(EntityTypeBuilder<CV> builder)
    {
        builder.ToTable("cvs");

        builder.HasKey(cv => cv.Id);

        builder.Property(cv => cv.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(cv => cv.CandidateId)
            .HasColumnName("candidate_id")
            .IsRequired();

        builder.Property(cv => cv.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(cv => cv.FilePath)
            .HasColumnName("file_path")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(cv => cv.FileSize)
            .HasColumnName("file_size");

        builder.Property(cv => cv.ParsedContent)
            .HasColumnName("parsed_content")
            .HasColumnType("text");

        builder.Property(cv => cv.Skills)
            .HasColumnName("skills")
            .HasColumnType("jsonb");

        builder.Property(cv => cv.Experience)
            .HasColumnName("experience")
            .HasColumnType("jsonb");

        builder.Property(cv => cv.Education)
            .HasColumnName("education")
            .HasColumnType("jsonb");

        builder.Property(cv => cv.IsPrimary)
            .HasColumnName("is_primary")
            .HasDefaultValue(false);

        builder.Property(cv => cv.UploadedAt)
            .HasColumnName("uploaded_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(cv => cv.CandidateId);

        // Relationships
        builder.HasMany(cv => cv.Applications)
            .WithOne(a => a.CV)
            .HasForeignKey(a => a.CVId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}