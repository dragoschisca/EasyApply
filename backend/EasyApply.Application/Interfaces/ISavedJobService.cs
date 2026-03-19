using EasyApply.Application.DTOs.SavedJob;

namespace EasyApply.Application.Interfaces.Services;

public interface ISavedJobService
{
    Task<SavedJobDto> GetByIdAsync(Guid id);
    Task<IEnumerable<SavedJobDto>> GetByCandidateIdAsync(Guid candidateId);
    Task<SavedJobDto> CreateAsync(CreateSavedJobDto dto);
    Task DeleteAsync(Guid id);
}
