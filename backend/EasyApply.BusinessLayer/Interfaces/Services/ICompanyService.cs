using EasyApply.BusinessLayer.Structure.DTOs.Company;

namespace EasyApply.BusinessLayer.Interfaces.Services;

public interface ICompanyService
{
    Task<CompanyDto> GetByIdAsync(Guid id);
    Task<CompanyDto> GetByUserIdAsync(Guid userId);
    Task<List<CompanyDto>> GetAllAsync(int page, int pageSize);
    Task<CompanyDto> CreateAsync(Guid userId, CreateCompanyDto dto);
    Task<CompanyDto> UpdateAsync(Guid userId, UpdateCompanyDto dto);
    Task DeleteAsync(Guid userId);
    Task<CompanyStatisticsDto> GetStatisticsAsync(Guid companyId);
    Task IncrementViewCountAsync(Guid id);
}
