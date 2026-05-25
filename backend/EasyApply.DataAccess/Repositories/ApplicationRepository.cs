using EasyApply.Domain.Entities;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.DataAccess.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Application entity)
    {
        await _context.Applications.AddAsync(entity);
    }

    public Task UpdateAsync(Application entity)
    {
        _context.Applications.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Application entity)
    {
        _context.Applications.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<Application?> GetByIdAsync(Guid id)
    {
        return await _context.Applications.FindAsync(id);
    }

    public async Task<IEnumerable<Application>> GetAllAsync()
    {
        return await _context.Applications.AsNoTracking().ToListAsync();
    }

    public async Task<(IEnumerable<Application> Items, int TotalCount)> GetPagedAsync(int skip, int take)
    {
        var total = await _context.Applications.CountAsync();
        var items = await _context.Applications.AsNoTracking().Skip(skip).Take(take).ToListAsync();
        return (items, total);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Application>> GetByCandidateIdAsync(Guid candidateId)
    {
        return await _context.Applications
            .Include(a => a.Job).ThenInclude(j => j.Company)
            .Include(a => a.Candidate).ThenInclude(c => c.User)
            .Include(a => a.CV)
            .AsNoTracking()
            .Where(a => a.CandidateId == candidateId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Application>> GetByJobIdAsync(Guid jobId)
    {
        return await _context.Applications
            .Include(a => a.Job).ThenInclude(j => j.Company)
            .Include(a => a.Candidate).ThenInclude(c => c.User)
            .Include(a => a.CV)
            .AsNoTracking()
            .Where(a => a.JobId == jobId)
            .ToListAsync();
    }

    public async Task<Application?> GetWithDetailsAsync(Guid id)
    {
        return await _context.Applications
            .Include(a => a.Job).ThenInclude(j => j.Company)
            .Include(a => a.Candidate).ThenInclude(c => c.User)
            .Include(a => a.CV)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> ExistsAsync(Guid candidateId, Guid jobId)
    {
        return await _context.Applications
            .AnyAsync(a => a.CandidateId == candidateId && a.JobId == jobId);
    }

    public async Task<int> GetCountByCompanyIdAsync(Guid companyId)
    {
        return await _context.Applications
            .CountAsync(a => a.Job.CompanyId == companyId);
    }

    public async Task<IEnumerable<ApplicationStatusHistory>> GetStatusTimelineAsync(Guid applicationId)
    {
        return await _context.ApplicationStatusHistories
            .AsNoTracking()
            .Where(h => h.ApplicationId == applicationId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }

    public async Task AddStatusHistoryAsync(ApplicationStatusHistory history)
    {
        await _context.ApplicationStatusHistories.AddAsync(history);
    }
}
