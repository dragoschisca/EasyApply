using EasyApply.Core.Entites;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.Infrastructure.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
}