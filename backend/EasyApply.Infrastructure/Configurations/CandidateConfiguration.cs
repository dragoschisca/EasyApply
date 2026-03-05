using EasyApply.Domains.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyApply.Infrastructure.Data.Configurations;
public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
{
    public void Configure(EntityTypeBuilder<Candidate> builder)
    {
        builder.ToTable("candidates");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(c => c.UserId)
            .IsUnique();

        builder.Property(c => c.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(c => c.Location)
            .HasColumnName("location")
            .HasMaxLength(255);

        builder.Property(c => c.LinkedInUrl)
            .HasColumnName("linkedin_url")
            .HasMaxLength(255);

        builder.Property(c => c.PortfolioUrl)
            .HasColumnName("portfolio_url")
            .HasMaxLength(255);

        builder.Property(c => c.Bio)
            .HasColumnName("bio")
            .HasColumnType("text");

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Ignore computed property
        builder.Ignore(c => c.FullName);

        // Relationships
        builder.HasMany(c => c.CVs)
            .WithOne(cv => cv.Candidate)
            .HasForeignKey(cv => cv.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Applications)
            .WithOne(a => a.Candidate)
            .HasForeignKey(a => a.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.SavedJobs)
            .WithOne(sj => sj.Candidate)
            .HasForeignKey(sj => sj.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}