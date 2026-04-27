using EasyApply.Domain.Entities;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.DataAccess.Repositories;

public class CVRepository : ICVRepository
{
    private readonly ApplicationDbContext _context;

    public CVRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CV entity)
    {
        await _context.CVs.AddAsync(entity);
    }

    public async Task UpdateAsync(CV entity)
    {
        _context.CVs.Update(entity);
    }

    public async Task DeleteAsync(CV entity)
    {
        _context.CVs.Remove(entity);
    }

    public async Task<CV?> GetByIdAsync(Guid id)
    {
        return await _context.CVs.FindAsync(id);
    }

    public async Task<IEnumerable<CV>> GetAllAsync()
    {
        return await _context.CVs.ToListAsync();
    }

    public async Task<(IEnumerable<CV> Items, int TotalCount)> GetPagedAsync(int skip, int take)
    {
        var total = await _context.CVs.CountAsync();
        var items = await _context.CVs.Skip(skip).Take(take).ToListAsync();
        return (items, total);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CV>> GetByCandidateIdAsync(Guid candidateId)
    {
        return await _context.CVs
            .Where(c => c.CandidateId == candidateId)
            .ToListAsync();
    }

    public async Task<CV?> GetPrimaryByCandidateIdAsync(Guid candidateId)
    {
        return await _context.CVs
            .FirstOrDefaultAsync(c => c.CandidateId == candidateId && c.IsPrimary);
    }

    public async Task SetPrimaryAsync(Guid cvId, Guid candidateId)
    {
        var cvs = await _context.CVs
            .Where(c => c.CandidateId == candidateId)
            .ToListAsync();

        foreach (var cv in cvs)
            cv.IsPrimary = cv.Id == cvId;

        await _context.SaveChangesAsync();
    }
}
