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

    public Task UpdateAsync(Job entity)
    {
        _context.Jobs.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Job entity)
    {
        _context.Jobs.Remove(entity);
        return Task.CompletedTask;
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
        var query = _context.Jobs.Include(j => j.Company).Where(j => j.CompanyId == companyId);
        if (activeOnly)
            query = query.Where(j => j.IsActive);
        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<Job> Jobs, int Total)> SearchAsync(
        string? keyword,
        string? location,
        string? category,
        string? employmentType,
        string? experienceLevel,
        int? locationType,
        decimal? minSalary,
        decimal? maxSalary,
        int page,
        int pageSize)
    {
        var query = _context.Jobs
            .Include(j => j.Company)
            .Where(j => j.IsActive)
            .AsQueryable();

        if (minSalary.HasValue)
            query = query.Where(j => !j.SalaryMax.HasValue || j.SalaryMax.Value >= minSalary.Value);

        if (maxSalary.HasValue)
            query = query.Where(j => !j.SalaryMin.HasValue || j.SalaryMin.Value <= maxSalary.Value);

        if (locationType.HasValue)
            query = query.Where(j => (int)j.LocationType == locationType.Value);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = $"%{keyword.ToLower()}%";
            query = query.Where(j =>
                EF.Functions.ILike(j.Title, lowerKeyword) ||
                EF.Functions.ILike(j.Description, lowerKeyword) ||
                EF.Functions.ILike(j.Requirements, lowerKeyword));
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            var lowerLocation = $"%{location.ToLower()}%";
            query = query.Where(j => j.Location != null && EF.Functions.ILike(j.Location, lowerLocation));
        }

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(j => j.Category == category);

        if (!string.IsNullOrWhiteSpace(employmentType) &&
            Enum.TryParse<EasyApply.Domain.Enums.WorkTime>(employmentType, true, out var et))
            query = query.Where(j => j.EmploymentType == et);

        if (!string.IsNullOrWhiteSpace(experienceLevel) &&
            Enum.TryParse<EasyApply.Domain.Enums.ExperienceLevel>(experienceLevel, true, out var el))
            query = query.Where(j => j.ExperienceLevel == el);

        var total = await query.CountAsync();
        var skip = (page - 1) * pageSize;
        var items = await query.OrderByDescending(j => j.CreatedAt).Skip(skip).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<IEnumerable<Job>> GetRecommendationsAsync(Guid jobId, int count)
    {
        var sourceJob = await _context.Jobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId);
        if (sourceJob == null) return Enumerable.Empty<Job>();

        return await _context.Jobs
            .Include(j => j.Company)
            .Where(j => j.Id != jobId && j.IsActive && 
                       (j.CompanyId == sourceJob.CompanyId || j.Category == sourceJob.Category))
            .OrderByDescending(j => j.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Job>> GetNearbyAsync(double latitude, double longitude, double radiusKm)
    {
        double deltaLat = radiusKm / 111.0;
        double deltaLon = radiusKm / (111.0 * Math.Cos(latitude * Math.PI / 180.0));

        var candidates = await _context.Jobs
            .Include(j => j.Company)
            .Where(j =>
                j.IsActive &&
                j.Latitude.HasValue &&
                j.Longitude.HasValue &&
                j.Latitude.Value >= latitude - deltaLat &&
                j.Latitude.Value <= latitude + deltaLat &&
                j.Longitude.Value >= longitude - deltaLon &&
                j.Longitude.Value <= longitude + deltaLon)
            .ToListAsync();

        return candidates.Where(j => Haversine(latitude, longitude, j.Latitude!.Value, j.Longitude!.Value) <= radiusKm);
    }

    public async Task IncrementViewCountAsync(Guid jobId, bool saveChanges = true)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job != null)
        {
            job.ViewsCount++;
            if (saveChanges) await _context.SaveChangesAsync();
        }
    }

    public async Task AddJobViewAsync(JobView view)
    {
        await _context.JobViews.AddAsync(view);
    }

    public async Task<IEnumerable<Job>> GetTopJobsByCompanyIdAsync(Guid companyId, int count)
    {
        return await _context.Jobs
            .Where(j => j.CompanyId == companyId)
            .OrderByDescending(j => j.ViewsCount + (j.ViewsCount > 0 ? (double)j.ApplicationsCount / j.ViewsCount * 1000 : 0))
            .Take(count)
            .ToListAsync();
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    public async Task<IEnumerable<(decimal? Min, decimal? Max)>> GetSalaryBenchmarkDataAsync(string category, string experienceLevel)
    {
        var query = _context.Jobs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(j => j.Category == category);

        if (!string.IsNullOrWhiteSpace(experienceLevel) &&
            Enum.TryParse<EasyApply.Domain.Enums.ExperienceLevel>(experienceLevel, true, out var el))
            query = query.Where(j => j.ExperienceLevel == el);

        return await query
            .Where(j => j.SalaryMin.HasValue || j.SalaryMax.HasValue)
            .Select(j => new ValueTuple<decimal?, decimal?>(j.SalaryMin, j.SalaryMax))
            .ToListAsync();
    }

    private static double ToRad(double deg) => deg * Math.PI / 180.0;
}