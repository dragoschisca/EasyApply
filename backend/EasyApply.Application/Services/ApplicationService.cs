using EasyApply.Application.DTOs.Application;
using EasyApply.Domains.Entities;
using EasyApply.Domains.Enums;
using EasyApply.Domains.Exceptions;
using EasyApply.Domains.Interfaces.Repositories;
using EasyApply.Application.Interfaces.Services;

namespace EasyApply.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IJobRepository _jobRepository;

    public ApplicationService(IApplicationRepository applicationRepository, IJobRepository jobRepository)
    {
        _applicationRepository = applicationRepository;
        _jobRepository = jobRepository;
    }

    public async Task<ApplicationDto> GetByIdAsync(Guid id)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(id);
        if (application == null) throw new NotFoundException($"Application with ID {id} not found.");
        return MapToDto(application);
    }

    public async Task<IEnumerable<ApplicationDto>> GetByCandidateIdAsync(Guid candidateId)
    {
        var applications = await _applicationRepository.GetByCandidateIdAsync(candidateId);
        return applications.Select(MapToDto);
    }

    public async Task<IEnumerable<ApplicationDto>> GetByJobIdAsync(Guid jobId)
    {
        var applications = await _applicationRepository.GetByJobIdAsync(jobId);
        return applications.Select(MapToDto);
    }

    public async Task<ApplicationDto> CreateAsync(Guid candidateId, CreateApplicationDto dto)
    {
        var job = await _jobRepository.GetByIdAsync(dto.JobId);
        if (job == null) throw new NotFoundException($"Job with ID {dto.JobId} not found.");

        var exists = await _applicationRepository.ExistsAsync(candidateId, dto.JobId);
        if (exists) throw new BusinessException("You have already applied to this job.");

        var application = new EasyApply.Domains.Entities.Application
        {
            Id = Guid.NewGuid(),
            CandidateId = candidateId,
            JobId = dto.JobId,
            CVId = dto.CVId,
            Status = ApplicationStatus.Pending,
            AppliedAt = DateTime.UtcNow
        };

        await _applicationRepository.AddAsync(application);
        await _applicationRepository.SaveChangesAsync();

        return MapToDto(application);
    }

    public async Task<ApplicationDto> UpdateStatusAsync(Guid id, UpdateApplicationStatusDto dto)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application == null) throw new NotFoundException($"Application with ID {id} not found.");

        if (Enum.TryParse<ApplicationStatus>(dto.Status, true, out var status))
        {
            application.Status = status;
        }
        else
        {
            throw new BusinessException($"Invalid application status '{dto.Status}'.");
        }

        application.ReviewedAt = DateTime.UtcNow;

        await _applicationRepository.UpdateAsync(application);
        await _applicationRepository.SaveChangesAsync();

        return MapToDto(application);
    }

    public async Task DeleteAsync(Guid id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application == null) throw new NotFoundException($"Application with ID {id} not found.");

        await _applicationRepository.DeleteAsync(application);
        await _applicationRepository.SaveChangesAsync();
    }

    private static ApplicationDto MapToDto(EasyApply.Domains.Entities.Application application)
    {
        return new ApplicationDto
        {
            Id = application.Id,
            CandidateId = application.CandidateId,
            JobId = application.JobId,
            JobTitle = application.Job?.Title ?? string.Empty,
            CompanyName = application.Job?.Company?.CompanyName ?? string.Empty,
            CVFileName = application.CV?.FileName ?? string.Empty,
            Status = application.Status.ToString(),
            CompatibilityScore = (double?)(application.CompatibilityScore),
            CreatedAt = application.AppliedAt,
            UpdatedAt = application.ReviewedAt ?? application.AppliedAt
        };
    }
}
