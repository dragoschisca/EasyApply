using EasyApply.Application.DTOs.Candidate;

namespace EasyApply.Application.Interfaces.Services;

public interface ICandidateService
{
    Task<CandidateDto> GetByIdAsync(Guid id);

    Task<CandidateDto> GetByUserIdAsync(Guid userId);

    Task<List<CandidateDto>> GetAllAsync(int page, int pageSize);

    Task<CandidateDto> CreateAsync(Guid userId, CreateCandidateDto dto);

    Task<CandidateDto> UpdateAsync(Guid userId, UpdateCandidateDto dto);

    Task DeleteAsync(Guid userId);

    Task<List<CandidateDto>> SearchAsync(string searchTerm);
}