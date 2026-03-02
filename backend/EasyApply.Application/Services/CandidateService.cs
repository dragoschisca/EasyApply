using AutoMapper;
using EasyApplyAPI.DTOs.Candidate;

namespace RecruitmentPlatform.Application.Services;

public class CandidateService : ICandidateService
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly IMapper _mapper;

    public CandidateService(
        ICandidateRepository candidateRepository,
        IMapper mapper)
    {
        _candidateRepository = candidateRepository;
        _mapper = mapper;
    }

    public async Task<CandidateDto> GetByIdAsync(Guid id)
    {
        var candidate = await _candidateRepository.GetWithDetailsAsync(id);
        if (candidate == null)
            throw new NotFoundException($"Candidate with ID {id} not found.");

        return _mapper.Map<CandidateDto>(candidate);
    }

    public async Task<CandidateDto> GetByUserIdAsync(Guid userId)
    {
        var candidate = await _candidateRepository.GetByUserIdAsync(userId);
        if (candidate == null)
            throw new NotFoundException("Candidate profile not found.");

        return _mapper.Map<CandidateDto>(candidate);
    }

    public async Task<PagedResultDto<CandidateDto>> GetAllAsync(PaginationDto pagination)
    {
        var (candidates, totalCount) =
            await _candidateRepository.GetPagedAsync(
                pagination.Skip,
                pagination.PageSize);

        var candidateDtos = _mapper.Map<List<CandidateDto>>(candidates);

        return PagedResultDto<CandidateDto>.Create(
            candidateDtos,
            totalCount,
            pagination.Page,
            pagination.PageSize);
    }

    public async Task<CandidateDto> CreateAsync(Guid userId, CreateCandidateDto dto)
    {
        var existing = await _candidateRepository.GetByUserIdAsync(userId);
        if (existing != null)
            throw new BusinessException("Candidate profile already exists for this user.");

        var candidate = _mapper.Map<Core.Entities.Candidate>(dto);
        candidate.UserId = userId;
        candidate.CreatedAt = DateTime.UtcNow;
        candidate.UpdatedAt = DateTime.UtcNow;

        await _candidateRepository.AddAsync(candidate);
        await _candidateRepository.SaveChangesAsync();

        return _mapper.Map<CandidateDto>(candidate);
    }

    public async Task<CandidateDto> UpdateAsync(Guid userId, UpdateCandidateDto dto)
    {
        var candidate = await _candidateRepository.GetByUserIdAsync(userId);
        if (candidate == null)
            throw new NotFoundException("Candidate profile not found.");

        if (!string.IsNullOrWhiteSpace(dto.FirstName))
            candidate.FirstName = dto.FirstName;

        if (!string.IsNullOrWhiteSpace(dto.LastName))
            candidate.LastName = dto.LastName;

        if (dto.Phone != null)
            candidate.Phone = dto.Phone;

        if (dto.Location != null)
            candidate.Location = dto.Location;

        if (dto.LinkedInUrl != null)
            candidate.LinkedInUrl = dto.LinkedInUrl;

        if (dto.PortfolioUrl != null)
            candidate.PortfolioUrl = dto.PortfolioUrl;

        if (dto.Bio != null)
            candidate.Bio = dto.Bio;

        candidate.UpdatedAt = DateTime.UtcNow;

        _candidateRepository.Update(candidate);
        await _candidateRepository.SaveChangesAsync();

        return _mapper.Map<CandidateDto>(candidate);
    }

    public async Task DeleteAsync(Guid userId)
    {
        var candidate = await _candidateRepository.GetByUserIdAsync(userId);
        if (candidate == null)
            throw new NotFoundException("Candidate profile not found.");

        _candidateRepository.Delete(candidate);
        await _candidateRepository.SaveChangesAsync();
    }

    public async Task<List<CandidateDto>> SearchAsync(string searchTerm)
    {
        var candidates = await _candidateRepository.SearchAsync(searchTerm);
        return _mapper.Map<List<CandidateDto>>(candidates);
    }
}