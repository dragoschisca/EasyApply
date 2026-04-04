using EasyApply.Domain.Entities;

namespace EasyApply.Domain.Models.Interfaces.Repositories;

public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<Company?> GetByUserIdAsync(Guid userId);
    Task<Company?> GetWithDetailsAsync(Guid id);
}
