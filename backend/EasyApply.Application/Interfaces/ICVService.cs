using EasyApply.Application.DTOs.CV;

namespace EasyApply.Application.Interfaces.Services;

public interface ICVService
{
    Task<CVDto> GetByIdAsync(Guid id);
    Task<IEnumerable<CVDto>> GetByCandidateIdAsync(Guid candidateId);
    Task<CVDto?> GetPrimaryByCandidateIdAsync(Guid candidateId);
    Task<CVDto> CreateAsync(Guid candidateId, CreateCVDto dto);
    Task<CVDto> UpdateAsync(Guid id, UpdateCVDto dto);
    Task DeleteAsync(Guid id);
    Task SetPrimaryAsync(Guid id, Guid candidateId);
}
