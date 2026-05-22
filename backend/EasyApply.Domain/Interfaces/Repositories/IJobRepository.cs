using EasyApply.Domain.Entities;
using EasyApply.Domain.Models.Job;

namespace EasyApply.Domain.Interfaces.Repositories;

public interface IJobRepository : IBaseRepository<Job>
{
    Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId, bool activeOnly = true);
    Task<(IEnumerable<Job> Jobs, int Total)> SearchAsync(
        string? keyword, string? location, string? category, string? employmentType,
        string? experienceLevel, int? locationType, decimal? minSalary, decimal? maxSalary, int page, int pageSize);
    Task<(IEnumerable<Job> Items, int TotalCount)> SearchAsync(SearchJobDto searchDto);
    Task IncrementViewCountAsync(Guid jobId, bool saveChanges = true);
    Task<IEnumerable<Job>> GetNearbyAsync(double latitude, double longitude, double radiusKm);
    Task<IEnumerable<Job>> GetRecommendationsAsync(Guid jobId, int count);
    Task<IEnumerable<(decimal? Min, decimal? Max)>> GetSalaryBenchmarkDataAsync(string category, string experienceLevel);
    Task AddJobViewAsync(JobView view);
    Task<IEnumerable<Job>> GetTopJobsByCompanyIdAsync(Guid companyId, int count);
}