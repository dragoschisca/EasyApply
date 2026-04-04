using EasyApply.Domain.Entities;

namespace EasyApply.Domain.Models.Interfaces.Repositories;

public interface IJobRepository : IBaseRepository<Job>
{
    Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId, bool activeOnly = true);
    Task<(IEnumerable<Job> Jobs, int Total)> SearchAsync(
        string? keyword, string? location, string? employmentType,
        string? experienceLevel, bool? isRemote, int page, int pageSize);
    Task IncrementViewCountAsync(Guid jobId);
}