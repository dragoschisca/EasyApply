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
    }
}