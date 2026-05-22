using EasyApply.BusinessLayer.Structure.DTOs.SavedJob;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Exceptions;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.BusinessLayer.Interfaces.Services;

namespace EasyApply.BusinessLayer.Core;

public class SavedJobService : ISavedJobService
{
    private readonly ISavedJobRepository _savedJobRepository;
    private readonly IJobRepository _jobRepository;
    private readonly ICandidateRepository _candidateRepository;

    public SavedJobService(
        ISavedJobRepository savedJobRepository, 
        IJobRepository jobRepository, 
        ICandidateRepository candidateRepository)
    {
        _savedJobRepository = savedJobRepository;
        _jobRepository = jobRepository;
        _candidateRepository = candidateRepository;
    }

    public async Task<SavedJobDto> GetByIdAsync(Guid id)
    {
        var savedJob = await _savedJobRepository.GetByIdAsync(id);
        if (savedJob == null) throw new NotFoundException($"SavedJob with ID {id} not found.");
        return MapToDto(savedJob);
    }

    public async Task<IEnumerable<SavedJobDto>> GetByCandidateIdAsync(Guid candidateId)
    {
        var savedJobs = await _savedJobRepository.GetByCandidateIdAsync(candidateId);
        return savedJobs.Select(MapToDto);
    }

    public async Task<SavedJobDto> CreateAsync(CreateSavedJobDto dto)
    {
        var candidate = await _candidateRepository.GetByIdAsync(dto.CandidateId);
        if (candidate == null) throw new NotFoundException($"Candidate with ID {dto.CandidateId} not found.");

        var job = await _jobRepository.GetByIdAsync(dto.JobId);
        if (job == null) throw new NotFoundException($"Job with ID {dto.JobId} not found.");

        var exists = await _savedJobRepository.ExistsAsync(dto.CandidateId, dto.JobId);
        if (exists) throw new BusinessException("Job is already saved for this candidate.");

        var savedJob = new SavedJob
        {
            Id = Guid.NewGuid(),
            CandidateId = dto.CandidateId,
            JobId = dto.JobId,
            SavedAt = DateTime.UtcNow
        };

        await _savedJobRepository.AddAsync(savedJob);
        await _savedJobRepository.SaveChangesAsync();

        return MapToDto(savedJob);
    }

    public async Task DeleteAsync(Guid id)
    {
        var savedJob = await _savedJobRepository.GetByIdAsync(id);
        if (savedJob == null) throw new NotFoundException($"SavedJob with ID {id} not found.");

        await _savedJobRepository.DeleteAsync(savedJob);
        await _savedJobRepository.SaveChangesAsync();
    }

    private static SavedJobDto MapToDto(SavedJob savedJob)
    {
        return new SavedJobDto
        {
            Id = savedJob.Id,
            CandidateId = savedJob.CandidateId,
            JobId = savedJob.JobId,
            SavedAt = savedJob.SavedAt
        };
    }
}
