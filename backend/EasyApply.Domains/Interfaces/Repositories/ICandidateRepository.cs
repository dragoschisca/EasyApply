using EasyApply.Core.Entites;

namespace EasyApply.Domains.Interfaces.Repositories;

public interface ICandidateRepository : IBaseRepository<Candidate>
{
    Task<Candidate?> GetByUserIdAsync(Guid userId);
    Task<Candidate?> GetWithDetailsAsync(Guid id);
}