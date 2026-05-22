using EasyApply.Domain.Entities;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.DataAccess.Repositories;

public class SavedJobRepository : ISavedJobRepository
{
    private readonly ApplicationDbContext _context;

    public SavedJobRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SavedJob entity)
    {
        await _context.SavedJobs.AddAsync(entity);
    }

    public Task UpdateAsync(SavedJob entity)
    {
        _context.SavedJobs.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(SavedJob entity)
    {
        _context.SavedJobs.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<SavedJob?> GetByIdAsync(Guid id)
    {
        return await _context.SavedJobs.FindAsync(id);
    }

    public async Task<IEnumerable<SavedJob>> GetAllAsync()
    {
        return await _context.SavedJobs.ToListAsync();
    }

    public async Task<(IEnumerable<SavedJob> Items, int TotalCount)> GetPagedAsync(int skip, int take)
    {
        var total = await _context.SavedJobs.CountAsync();
        var items = await _context.SavedJobs.Skip(skip).Take(take).ToListAsync();
        return (items, total);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<SavedJob>> GetByCandidateIdAsync(Guid candidateId)
    {
        return await _context.SavedJobs
            .Include(s => s.Job).ThenInclude(j => j.Company)
            .Where(s => s.CandidateId == candidateId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid candidateId, Guid jobId)
    {
        return await _context.SavedJobs
            .AnyAsync(s => s.CandidateId == candidateId && s.JobId == jobId);
    }
}
