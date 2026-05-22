using EasyApply.Domain.Entities;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.DataAccess.Repositories;

public class CandidateRepository : ICandidateRepository
{
    private readonly ApplicationDbContext _context;

    public CandidateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Candidate entity)
    {
        await _context.Candidates.AddAsync(entity);
    }

    public Task UpdateAsync(Candidate entity)
    {
        _context.Candidates.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Candidate entity)
    {
        _context.Candidates.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<Candidate?> GetByIdAsync(Guid id)
    {
        return await _context.Candidates.FindAsync(id);
    }

    public async Task<IEnumerable<Candidate>> GetAllAsync()
    {
        return await _context.Candidates.ToListAsync();
    }

    public async Task<(IEnumerable<Candidate> Items, int TotalCount)> GetPagedAsync(int skip, int take)
    {
        var total = await _context.Candidates.CountAsync();
        var items = await _context.Candidates.Skip(skip).Take(take).ToListAsync();
        return (items, total);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Candidate?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Candidates
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Candidate?> GetWithDetailsAsync(Guid id)
    {
        return await _context.Candidates
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Candidate>> SearchAsync(string term)
    {
        var pattern = $"%{term}%";
        return await _context.Candidates
            .Where(c => EF.Functions.ILike(c.FirstName, pattern) || EF.Functions.ILike(c.LastName, pattern))
            .ToListAsync();
    }
}