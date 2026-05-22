using EasyApply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyApply.DataAccess.Data.Configurations;

public class ApplicationStatusHistoryConfiguration : IEntityTypeConfiguration<ApplicationStatusHistory>
{
    public void Configure(EntityTypeBuilder<ApplicationStatusHistory> builder)
    {
        builder.ToTable("application_status_history");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(h => h.ApplicationId)
            .HasColumnName("application_id")
            .IsRequired();

        builder.Property(h => h.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(h => h.ChangedAt)
            .HasColumnName("changed_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(h => h.Feedback)
            .HasColumnName("feedback");

        builder.Property(h => h.ChangedBy)
            .HasColumnName("changed_by")
            .HasMaxLength(255);

        // Navigation
        builder.HasOne(h => h.Application)
            .WithMany()
            .HasForeignKey(h => h.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for performance: (application_id, changed_at DESC)
        builder.HasIndex(h => new { h.ApplicationId, h.ChangedAt })
            .HasDatabaseName("IX_application_status_history_application_id_changed_at")
            .IsDescending(false, true);
    }
}
