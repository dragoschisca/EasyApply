using EasyApply.BusinessLayer.Structure.DTOs.CV;

namespace EasyApply.BusinessLayer.Interfaces.Services;

public interface ICVService
{
    Task<CVDto> GetByIdAsync(Guid id);
    Task<IEnumerable<CVDto>> GetByCandidateIdAsync(Guid candidateId);
    Task<CVDto?> GetPrimaryByCandidateIdAsync(Guid candidateId);
    Task<CVDto> CreateAsync(Guid candidateId, string fileName, System.IO.Stream fileStream, long fileLength, bool isPrimary);
    Task<CVDto> UpdateAsync(Guid id, UpdateCVDto dto);
    Task DeleteAsync(Guid id);
    Task SetPrimaryAsync(Guid id, Guid candidateId);
}
