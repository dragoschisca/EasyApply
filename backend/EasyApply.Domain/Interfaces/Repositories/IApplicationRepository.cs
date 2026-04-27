using EasyApply.Domain.Entities;

namespace EasyApply.Domain.Interfaces.Repositories;

public interface IApplicationRepository : IBaseRepository<Application>
{
    Task<IEnumerable<Application>> GetByCandidateIdAsync(Guid candidateId);
    Task<IEnumerable<Application>> GetByJobIdAsync(Guid jobId);
    Task<Application?> GetWithDetailsAsync(Guid id);
    Task<bool> ExistsAsync(Guid candidateId, Guid jobId);
}