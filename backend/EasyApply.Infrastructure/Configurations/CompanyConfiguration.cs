using EasyApply.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyApply.Infrastructure.Data.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(c => c.UserId)
            .IsUnique();

        builder.Property(c => c.CompanyName)
            .HasColumnName("company_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Industry)
            .HasColumnName("industry")
            .HasMaxLength(100);

        builder.Property(c => c.CompanySize)
            .HasColumnName("company_size")
            .HasMaxLength(50);

        builder.Property(c => c.Website)
            .HasColumnName("website")
            .HasMaxLength(255);

        builder.Property(c => c.Description)
            .HasColumnName("description")
            .HasColumnType("text");

        builder.Property(c => c.LogoUrl)
            .HasColumnName("logo_url")
            .HasMaxLength(500);

        builder.Property(c => c.Location)
            .HasColumnName("location")
            .HasMaxLength(255);

        builder.Property(c => c.SubscriptionTier)
            .HasColumnName("subscription_tier")
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasDefaultValue(Domains.Enums.SubscriptionTier.Free);

        builder.Property(c => c.SubscriptionExpiresAt)
            .HasColumnName("subscription_expires_at");

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationships
        builder.HasMany(c => c.Jobs)
            .WithOne(j => j.Company)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}