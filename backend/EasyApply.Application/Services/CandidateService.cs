using EasyApply.Application.DTOs.Candidate;
using EasyApply.Domains.Entities;
using EasyApply.Domains.Enums;
using EasyApply.Domains.Exceptions;
using EasyApply.Domains.Interfaces.Repositories;
using EasyApply.Application.Interfaces.Services;
using EasyApply.Domains.Interfaces.Services;

namespace EasyApply.Application.Services;

public class CandidateService : ICandidateService
{
    private readonly ICandidateRepository _candidateRepository;

    public CandidateService(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
    }

    public async Task<CandidateDto> GetByIdAsync(Guid id)
    {
        var candidate = await _candidateRepository.GetWithDetailsAsync(id);

        if (candidate == null)
            throw new NotFoundException($"Candidate with ID {id} not found.");

        return MapToDto(candidate);
    }
    public async Task<CandidateDto> GetByUserIdAsync(Guid userId)
    {
        var candidate = await _candidateRepository.GetByUserIdAsync(userId);

        if (candidate == null)
            throw new NotFoundException("Candidate profile not found.");

        return MapToDto(candidate);
    }
    public async Task<List<CandidateDto>> GetAllAsync(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;

        var (candidates, _) =
            await _candidateRepository.GetPagedAsync(skip, pageSize);

        return candidates.Select(MapToDto).ToList();
    }
    public async Task<CandidateDto> CreateAsync(Guid userId, CreateCandidateDto dto)
    {
        var existing = await _candidateRepository.GetByUserIdAsync(userId);

        if (existing != null)
            throw new BusinessException("Candidate profile already exists for this user.");

        var candidate = new Candidate
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            Location = dto.Location,
            LinkedInUrl = dto.LinkedInUrl,
            PortfolioUrl = dto.PortfolioUrl,
            Bio = dto.Bio,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _candidateRepository.AddAsync(candidate);
        await _candidateRepository.SaveChangesAsync();

        return MapToDto(candidate);
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

        await _candidateRepository.UpdateAsync(candidate);
        await _candidateRepository.SaveChangesAsync();

        return MapToDto(candidate);
    }

    public async Task DeleteAsync(Guid userId)
    {
        var candidate = await _candidateRepository.GetByUserIdAsync(userId);

        if (candidate == null)
            throw new NotFoundException("Candidate profile not found.");

        await _candidateRepository.DeleteAsync(candidate);
        await _candidateRepository.SaveChangesAsync();
    }

    public async Task<List<CandidateDto>> SearchAsync(string searchTerm)
    {
        var candidates = await _candidateRepository.SearchAsync(searchTerm);

        return candidates.Select(MapToDto).ToList();
    }
    
    private static CandidateDto MapToDto(Candidate candidate)
    {
        return new CandidateDto
        {
            Id = candidate.Id,
            UserId = candidate.UserId,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Phone = candidate.Phone,
            Location = candidate.Location,
            LinkedInUrl = candidate.LinkedInUrl,
            PortfolioUrl = candidate.PortfolioUrl,
            Bio = candidate.Bio,
            CreatedAt = candidate.CreatedAt,
        };
    }
}