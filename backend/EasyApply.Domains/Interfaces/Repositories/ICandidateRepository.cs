
using EasyApply.Domains.Enums;

namespace EasyApply.Domains.Interfaces.Repositories;

public interface ICandidateRepository : IBaseRepository<Candidate>
{
    Task<Candidate?> GetByUserIdAsync(Guid userId);
    Task<Candidate?> GetWithDetailsAsync(Guid id);
    Task<IEnumerable<Candidate>> SearchAsync(string searchTerm);
}