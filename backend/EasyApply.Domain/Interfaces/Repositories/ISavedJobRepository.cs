using EasyApply.Domain.Entities;

namespace EasyApply.Domain.Interfaces.Repositories;

public interface ISavedJobRepository : IBaseRepository<SavedJob>
{
    Task<IEnumerable<SavedJob>> GetByCandidateIdAsync(Guid candidateId);
    Task<bool> ExistsAsync(Guid candidateId, Guid jobId);
}
