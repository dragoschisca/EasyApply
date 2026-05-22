using EasyApply.BusinessLayer.Structure.DTOs.Job;

namespace EasyApply.BusinessLayer.Interfaces.Services;

public interface IJobService
{
    Task<JobDto> GetByIdAsync(Guid id);
    Task<IEnumerable<JobDto>> GetByCompanyIdAsync(Guid companyId, bool activeOnly = true);
    Task<(IEnumerable<JobDto> Jobs, int Total)> SearchAsync(
        string? keyword, string? location, string? category, string? employmentType,
        string? experienceLevel, int? locationType, decimal? minSalary, decimal? maxSalary, int page, int pageSize);
    Task<SearchJobResultDto> SearchAsync(SearchJobDto searchDto);
    Task<IEnumerable<JobDto>> GetNearbyAsync(double latitude, double longitude, double radiusKm);
    Task<JobDto> CreateAsync(CreateJobDto dto);
    Task<JobDto> UpdateAsync(Guid id, UpdateJobDto dto);
    Task DeleteAsync(Guid id);
    Task IncrementViewCountAsync(Guid id);
    Task<IEnumerable<JobDto>> GetRecommendationsAsync(Guid id, int count);
    Task<SalaryBenchmarkResponse> GetSalaryBenchmarkAsync(SalaryBenchmarkRequest request);
}