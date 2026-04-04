
using EasyApply.Domain.Entities;

namespace EasyApply.Domain.Models.Interfaces.Repositories;

public interface ICandidateRepository : IBaseRepository<Candidate>
{
    Task<Candidate?> GetByUserIdAsync(Guid userId);
    Task<Candidate?> GetWithDetailsAsync(Guid id);
    Task<IEnumerable<Candidate>> SearchAsync(string searchTerm);
}