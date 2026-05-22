using EasyApply.Domain.Entities;

namespace EasyApply.Domain.Interfaces.Repositories;

public interface ICVRepository : IBaseRepository<CV>
{
    Task<IEnumerable<CV>> GetByCandidateIdAsync(Guid candidateId);
    Task<CV?> GetPrimaryByCandidateIdAsync(Guid candidateId);
    Task SetPrimaryAsync(Guid cvId, Guid candidateId);
}