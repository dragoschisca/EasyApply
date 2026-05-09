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

    /// <summary>
    /// Returns active jobs within <paramref name="radiusKm"/> kilometres of the given point,
    /// using the Haversine formula implemented in-process (no PostGIS required).
    /// Only jobs that already have stored coordinates are considered.
    /// </summary>
    public async Task<IEnumerable<Job>> GetNearbyAsync(double latitude, double longitude, double radiusKm)
    {
        // Pull jobs that have coordinates — EF can't translate Math.Sin/Cos to SQL,
        // so we filter the bounding box in SQL and apply the precise Haversine in memory.
        double deltaLat = radiusKm / 111.0;           // ~1° lat ≈ 111 km
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

        // Precise Haversine filter in memory.
        return candidates.Where(j => Haversine(latitude, longitude, j.Latitude!.Value, j.Longitude!.Value) <= radiusKm);
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

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>Returns the great-circle distance in kilometres between two coordinates.</summary>
    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0; // Earth's mean radius in km
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