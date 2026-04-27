
using EasyApply.Domain.Entities;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.DataAccess.Repositories;

public class JobRepository : IJobRepository
{
    private readonly ApplicationDbContext _context;

    public JobRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Job entity)
    {
        await _context.Jobs.AddAsync(entity);
    }

    public async Task UpdateAsync(Job entity)
    {
        _context.Jobs.Update(entity);
    }

    public async Task DeleteAsync(Job entity)
    {
        _context.Jobs.Remove(entity);
    }

    public async Task<Job?> GetByIdAsync(Guid id)
    {
        return await _context.Jobs
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<IEnumerable<Job>> GetAllAsync()
    {
        return await _context.Jobs.Include(j => j.Company).ToListAsync();
    }

    public async Task<(IEnumerable<Job> Items, int TotalCount)> GetPagedAsync(int skip, int take)
    {
        var total = await _context.Jobs.CountAsync();
        var items = await _context.Jobs.Include(j => j.Company).Skip(skip).Take(take).ToListAsync();
        return (items, total);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId, bool activeOnly = true)
    {
        var query = _context.Jobs.Where(j => j.CompanyId == companyId);
        if (activeOnly)
            query = query.Where(j => j.IsActive);
        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<Job> Jobs, int Total)> SearchAsync(
        string? keyword,
        string? location,
        string? employmentType,
        string? experienceLevel,
        bool? isRemote,
        int page,
        int pageSize)
    {
        var query = _context.Jobs
            .Include(j => j.Company)
            .Where(j => j.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(j =>
                j.Title.ToLower().Contains(lowerKeyword) ||
                j.Description.ToLower().Contains(lowerKeyword) ||
                j.Requirements.ToLower().Contains(lowerKeyword));
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            var lowerLocation = location.ToLower();
            query = query.Where(j => j.Location != null && j.Location.ToLower().Contains(lowerLocation));
        }

        if (!string.IsNullOrWhiteSpace(employmentType) &&
            Enum.TryParse<EasyApply.Domain.Enums.WorkTime>(employmentType, true, out var et))
            query = query.Where(j => j.EmploymentType == et);

        if (!string.IsNullOrWhiteSpace(experienceLevel) &&
            Enum.TryParse<EasyApply.Domain.Enums.ExperienceLevel>(experienceLevel, true, out var el))
            query = query.Where(j => j.ExperienceLevel == el);

        if (isRemote.HasValue)
            query = query.Where(j => j.IsRemote == isRemote.Value);

        var total = await query.CountAsync();
        var skip = (page - 1) * pageSize;
        var items = await query.OrderByDescending(j => j.CreatedAt).Skip(skip).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task IncrementViewCountAsync(Guid jobId)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job != null)
        {
            job.ViewsCount++;
            await _context.SaveChangesAsync();
        }
    }
}
