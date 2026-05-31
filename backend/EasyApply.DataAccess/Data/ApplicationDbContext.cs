using EasyApply.DataAccess.Data.Configurations;
using EasyApply.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.DataAccess.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<CV> CVs { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<SavedJob> SavedJobs { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<CompanyProfileView> CompanyProfileViews { get; set; }
    public DbSet<JobView> JobViews { get; set; }
    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }
    public DbSet<CompanyReview> CompanyReviews { get; set; }
    public DbSet<CompanyReviewHelpful> CompanyReviewHelpfuls { get; set; }
    public DbSet<CompanyRating> CompanyRatings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApplicationStatusHistoryConfiguration());

        modelBuilder.Entity<Application>(entity =>
        {
            entity.Property(a => a.RejectionFeedback).HasColumnName("rejection_feedback");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<CompanyProfileView>(entity =>
        {
            entity.HasIndex(v => v.CompanyId);
            entity.HasIndex(v => v.ViewedAt);
        });

        modelBuilder.Entity<JobView>(entity =>
        {
            entity.HasIndex(v => v.JobId);
            entity.HasIndex(v => v.ViewedAt);
        });

        modelBuilder.Entity<CompanyReview>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => new { r.CompanyId, r.CreatedAt });
            entity.HasIndex(r => new { r.UserId, r.CreatedAt });
            entity.HasIndex(r => new { r.UserId, r.CompanyId }).IsUnique(); // One review per user per company
            entity.HasQueryFilter(r => r.DeletedAt == null); // Apply soft delete query filter

            entity.HasOne(r => r.Company)
                .WithMany()
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CompanyReviewHelpful>(entity =>
        {
            entity.HasKey(h => new { h.ReviewId, h.UserId });

            entity.HasOne(h => h.Review)
                .WithMany()
                .HasForeignKey(h => h.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(h => h.User)
                .WithMany()
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CompanyRating>(entity =>
        {
            entity.HasKey(r => r.CompanyId);
            entity.Property(r => r.AverageRating).HasPrecision(3, 2);

            entity.HasOne(r => r.Company)
                .WithOne()
                .HasForeignKey<CompanyRating>(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}