using EasyApply.BusinessLayer.Structure.DTOs.Application;
using EasyApply.BusinessLayer.Structure.DTOs.AI;

namespace EasyApply.BusinessLayer.Interfaces.Services;

public interface IApplicationService
{
    Task<ApplicationDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ApplicationDto>> GetByCandidateIdAsync(Guid candidateId);
    Task<IEnumerable<ApplicationDto>> GetByJobIdAsync(Guid jobId);
    Task<ApplicationDto> CreateAsync(Guid candidateId, CreateApplicationDto dto);
    Task<ApplicationDto> UpdateStatusAsync(Guid id, UpdateApplicationStatusDto dto);
    Task DeleteAsync(Guid id);
    Task<ApplicationDto> AnalyzeAsync(Guid id);
}
