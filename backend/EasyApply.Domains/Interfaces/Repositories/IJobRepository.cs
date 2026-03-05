using EasyApply.Domains.Enums;

namespace EasyApply.Domains.Interfaces.Repositories;

public interface IJobRepository : EasyApply.Domains.Interfaces.Repositories.IBaseRepository<Job>
{
    Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId, bool activeOnly = true);
    Task<(IEnumerable<Job> Jobs, int Total)> SearchAsync(
        string? keyword, string? location, string? employmentType,
        string? experienceLevel, bool? isRemote, int page, int pageSize);
    Task IncrementViewCountAsync(Guid jobId);
}