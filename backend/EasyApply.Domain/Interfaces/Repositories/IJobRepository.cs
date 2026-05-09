using EasyApply.Domain.Entities;

namespace EasyApply.Domain.Interfaces.Repositories;

public interface IJobRepository : IBaseRepository<Job>
{
    Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId, bool activeOnly = true);
    Task<(IEnumerable<Job> Jobs, int Total)> SearchAsync(
        string? keyword, string? location, string? category, string? employmentType,
        string? experienceLevel, int? locationType, decimal? minSalary, decimal? maxSalary, int page, int pageSize);
    Task IncrementViewCountAsync(Guid jobId);
    Task<IEnumerable<Job>> GetNearbyAsync(double latitude, double longitude, double radiusKm);
    Task<IEnumerable<(decimal? Min, decimal? Max)>> GetSalaryBenchmarkDataAsync(string category, string experienceLevel);
}