using EasyApply.Domain.Entities;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User entity)
    {
        await _context.Users.AddAsync(entity);
    }

    public async Task UpdateAsync(User entity)
    {
        _context.Users.Update(entity);
    }

    public async Task DeleteAsync(User entity)
    {
        _context.Users.Remove(entity);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(int skip, int take)
    {
        var total = await _context.Users.CountAsync();
        var items = await _context.Users.Skip(skip).Take(take).ToListAsync();
        return (items, total);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    // public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    //  {
    //      return await _context.Users
    //          .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    //  }
}
