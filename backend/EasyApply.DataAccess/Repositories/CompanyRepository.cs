using EasyApply.Domain.Entities;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.DataAccess.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Company entity)
    {
        await _context.Companies.AddAsync(entity);
    }

    public Task UpdateAsync(Company entity)
    {
        _context.Companies.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Company entity)
    {
        _context.Companies.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<Company?> GetByIdAsync(Guid id)
    {
        return await _context.Companies.FindAsync(id);
    }

    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        return await _context.Companies.ToListAsync();
    }

    public async Task<(IEnumerable<Company> Items, int TotalCount)> GetPagedAsync(int skip, int take)
    {
        var total = await _context.Companies.CountAsync();
        var items = await _context.Companies.Skip(skip).Take(take).ToListAsync();
        return (items, total);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Company?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Company?> GetWithDetailsAsync(Guid id)
    {
        return await _context.Companies
            .Include(c => c.Jobs)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddProfileViewAsync(CompanyProfileView view)
    {
        await _context.CompanyProfileViews.AddAsync(view);
    }

    public async Task<IEnumerable<CompanyProfileView>> GetProfileViewsAsync(Guid companyId, DateTime since)
    {
        return await _context.CompanyProfileViews
            .Where(v => v.CompanyId == companyId && v.ViewedAt >= since)
            .ToListAsync();
    }
}
