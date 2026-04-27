using EasyApply.BusinessLayer.Structure.DTOs.CV;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Exceptions;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.BusinessLayer.Interfaces.Services;

namespace EasyApply.BusinessLayer.Core;

public class CVService : ICVService
{
    private readonly ICVRepository _cvRepository;
    private readonly ICandidateRepository _candidateRepository;

    public CVService(ICVRepository cvRepository, ICandidateRepository candidateRepository)
    {
        _cvRepository = cvRepository;
        _candidateRepository = candidateRepository;
    }

    public async Task<CVDto> GetByIdAsync(Guid id)
    {
        var cv = await _cvRepository.GetByIdAsync(id);
        if (cv == null) throw new NotFoundException($"CV with ID {id} not found.");
        return MapToDto(cv);
    }

    public async Task<IEnumerable<CVDto>> GetByCandidateIdAsync(Guid candidateId)
    {
        var cvs = await _cvRepository.GetByCandidateIdAsync(candidateId);
        return cvs.Select(MapToDto);
    }

    public async Task<CVDto?> GetPrimaryByCandidateIdAsync(Guid candidateId)
    {
        var cv = await _cvRepository.GetPrimaryByCandidateIdAsync(candidateId);
        return cv != null ? MapToDto(cv) : null;
    }

    public async Task<CVDto> CreateAsync(Guid candidateId, CreateCVDto dto)
    {
        var candidate = await _candidateRepository.GetWithDetailsAsync(candidateId);
        if (candidate == null) throw new NotFoundException($"Candidate with ID {candidateId} not found.");

        var cv = new CV
        {
            Id = Guid.NewGuid(),
            CandidateId = candidateId,
            FileName = dto.FileName,
            FilePath = dto.FilePath,
            FileSize = dto.FileSize,
            ParsedContent = dto.ParsedContent,
            Skills = dto.Skills,
            Experience = dto.Experience,
            Education = dto.Education,
            IsPrimary = dto.IsPrimary,
            UploadedAt = DateTime.UtcNow
        };

        await _cvRepository.AddAsync(cv);
        await _cvRepository.SaveChangesAsync();

        if (dto.IsPrimary)
        {
            await _cvRepository.SetPrimaryAsync(cv.Id, candidateId);
            await _cvRepository.SaveChangesAsync();
        }

        return MapToDto(cv);
    }

    public async Task<CVDto> UpdateAsync(Guid id, UpdateCVDto dto)
    {
        var cv = await _cvRepository.GetByIdAsync(id);
        if (cv == null) throw new NotFoundException($"CV with ID {id} not found.");

        if (!string.IsNullOrWhiteSpace(dto.FileName)) cv.FileName = dto.FileName;
        if (!string.IsNullOrWhiteSpace(dto.FilePath)) cv.FilePath = dto.FilePath;
        if (dto.FileSize.HasValue) cv.FileSize = dto.FileSize.Value;
        if (dto.ParsedContent != null) cv.ParsedContent = dto.ParsedContent;
        if (dto.Skills != null) cv.Skills = dto.Skills;
        if (dto.Experience != null) cv.Experience = dto.Experience;
        if (dto.Education != null) cv.Education = dto.Education;
        
        if (dto.IsPrimary.HasValue && dto.IsPrimary.Value)
        {
            await _cvRepository.SetPrimaryAsync(id, cv.CandidateId);
        }

        await _cvRepository.UpdateAsync(cv);
        await _cvRepository.SaveChangesAsync();

        return MapToDto(cv);
    }

    public async Task DeleteAsync(Guid id)
    {
        var cv = await _cvRepository.GetByIdAsync(id);
        if (cv == null) throw new NotFoundException($"CV with ID {id} not found.");

        await _cvRepository.DeleteAsync(cv);
        await _cvRepository.SaveChangesAsync();
    }

    public async Task SetPrimaryAsync(Guid id, Guid candidateId)
    {
        var cv = await _cvRepository.GetByIdAsync(id);
        if (cv == null || cv.CandidateId != candidateId) throw new NotFoundException($"CV with ID {id} not found for this candidate.");

        await _cvRepository.SetPrimaryAsync(id, candidateId);
        await _cvRepository.SaveChangesAsync();
    }

    private static CVDto MapToDto(CV cv)
    {
        return new CVDto
        {
            Id = cv.Id,
            CandidateId = cv.CandidateId,
            FileName = cv.FileName,
            FilePath = cv.FilePath,
            FileSize = cv.FileSize,
            ParsedContent = cv.ParsedContent,
            Skills = cv.Skills,
            Experience = cv.Experience,
            Education = cv.Education,
            IsPrimary = cv.IsPrimary,
            UploadedAt = cv.UploadedAt
        };
    }
}
