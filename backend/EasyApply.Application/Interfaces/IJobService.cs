using EasyApply.Application.DTOs.Job;

namespace EasyApply.Application.Interfaces.Services;

public interface IJobService
{
    Task<JobDto> GetByIdAsync(Guid id);
    Task<IEnumerable<JobDto>> GetByCompanyIdAsync(Guid companyId, bool activeOnly = true);
    Task<(IEnumerable<JobDto> Jobs, int Total)> SearchAsync(
        string? keyword, string? location, string? employmentType,
        string? experienceLevel, bool? isRemote, int page, int pageSize);
    Task<JobDto> CreateAsync(CreateJobDto dto);
    Task<JobDto> UpdateAsync(Guid id, UpdateJobDto dto);
    Task DeleteAsync(Guid id);
    Task IncrementViewCountAsync(Guid id);
}
