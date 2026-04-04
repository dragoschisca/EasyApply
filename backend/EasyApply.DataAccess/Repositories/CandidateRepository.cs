using EasyApply.Domain.Entities;
using EasyApply.Domain.Models.Interfaces.Repositories;
using EasyApply.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task UpdateAsync(Candidate entity)
    {
        _context.Candidates.Update(entity);
    }

    public async Task DeleteAsync(Candidate entity)
    {
        _context.Candidates.Remove(entity);
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
        return await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Candidate?> GetWithDetailsAsync(Guid id)
    {
        return await _context.Candidates.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Candidate>> SearchAsync(string term)
    {
        return await _context.Candidates
            .Where(c => c.FirstName.Contains(term) || c.LastName.Contains(term))
            .ToListAsync();
    }
}