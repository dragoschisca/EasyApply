using EasyApply.Domain.Entities;

namespace EasyApply.Domain.Interfaces.Repositories;

public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<Company?> GetByUserIdAsync(Guid userId);
    Task<Company?> GetWithDetailsAsync(Guid id);
    Task AddProfileViewAsync(CompanyProfileView view);
    Task<IEnumerable<CompanyProfileView>> GetProfileViewsAsync(Guid companyId, DateTime since);
}
