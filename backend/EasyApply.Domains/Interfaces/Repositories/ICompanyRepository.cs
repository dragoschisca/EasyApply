using EasyApply.Domains.Enums;

namespace EasyApply.Domains.Interfaces.Repositories;

public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<Company?> GetByUserIdAsync(Guid userId);
    Task<Company?> GetWithDetailsAsync(Guid id);
}
