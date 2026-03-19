using EasyApply.Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.Infrastructure.Data;

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
}