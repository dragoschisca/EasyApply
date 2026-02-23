using EasyApply.Core.Entites;
using Microsoft.EntityFrameworkCore;

namespace EasyApplyAPI.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
}