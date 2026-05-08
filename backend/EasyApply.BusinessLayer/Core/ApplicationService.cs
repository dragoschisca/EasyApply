using EasyApply.BusinessLayer.Structure.DTOs.Application;
using EasyApply.BusinessLayer.Structure.DTOs.AI;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Enums;
using EasyApply.Domain.Exceptions;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.BusinessLayer.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace EasyApply.BusinessLayer.Core;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IGeminiService _geminiService;
    private readonly ISupabaseStorageService _storageService;
    private readonly INotificationService _notificationService;
    private readonly string _cvBucket;

    public ApplicationService(
        IApplicationRepository applicationRepository, 
        IJobRepository jobRepository, 
        IGeminiService geminiService,
        ISupabaseStorageService storageService,
        INotificationService notificationService,
        IConfiguration configuration)
    {
        _applicationRepository = applicationRepository;
        _jobRepository = jobRepository;
        _geminiService = geminiService;
        _storageService = storageService;
        _notificationService = notificationService;
        _cvBucket = configuration["Supabase:CVBucket"] 
                    ?? Environment.GetEnvironmentVariable("SUPABASE_CV_BUCKET") 
                    ?? "cv-uploads";
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

        var application = new EasyApply.Domain.Entities.Application
        {
            Id = Guid.NewGuid(),
            CandidateId = candidateId,
            JobId = dto.JobId,
            CVId = dto.CVId,
            Status = ApplicationStatus.Pending,
            AppliedAt = DateTime.UtcNow
        };

        await _applicationRepository.AddAsync(application);
        
        // Increment application count on job
        job.ApplicationsCount++;
        await _jobRepository.UpdateAsync(job);
        
        await _applicationRepository.SaveChangesAsync();

        return MapToDto(application);
    }

    public async Task<ApplicationDto> UpdateStatusAsync(Guid id, UpdateApplicationStatusDto dto)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(id);
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

        // Notify candidate using their UserId (not CandidateId)
        if (application.Candidate != null)
        {
            var jobTitle = application.Job?.Title ?? "your application";
            await _notificationService.CreateNotificationAsync(
                application.Candidate.UserId,
                "Application Update",
                $"Your application for '{jobTitle}' has been updated to {status}.",
                $"/applications");
        }

        return MapToDto(application);
    }

    public async Task DeleteAsync(Guid id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application == null) throw new NotFoundException($"Application with ID {id} not found.");

        await _applicationRepository.DeleteAsync(application);
        await _applicationRepository.SaveChangesAsync();
    }

    public async Task<ApplicationDto> AnalyzeAsync(Guid id)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(id);
        if (application == null) throw new NotFoundException($"Application with ID {id} not found.");

        if (application.CV == null || string.IsNullOrEmpty(application.CV.FilePath))
            throw new BusinessException("Application has no CV to analyze.");

        if (application.Job == null)
            throw new BusinessException("Application has no Job associated.");

        // Download CV from Supabase
        using var cvStream = await _storageService.DownloadFileAsync(_cvBucket, application.CV.FilePath);

        var result = await _geminiService.GetCompatibilityResultAsync(
            cvStream,
            application.Job.Title,
            application.Job.Description,
            application.Job.RequiredSkills ?? string.Empty);

        application.CompatibilityScore = (decimal)result.Score;
        application.ScoreDetails = System.Text.Json.JsonSerializer.Serialize(result);
        
        await _applicationRepository.UpdateAsync(application);
        await _applicationRepository.SaveChangesAsync();

        return MapToDto(application);
    }

    private static ApplicationDto MapToDto(Application application)
    {
        var dto = new ApplicationDto
        {
            Id = application.Id,
            CandidateId = application.CandidateId,
            CandidateName = application.Candidate != null ? $"{application.Candidate.FirstName} {application.Candidate.LastName}" : "Anonymous",
            CandidateEmail = application.Candidate?.User?.Email ?? string.Empty,
            JobId = application.JobId,
            JobTitle = application.Job?.Title ?? string.Empty,
            CompanyName = application.Job?.Company?.CompanyName ?? string.Empty,
            CVFileName = application.CV?.FileName ?? string.Empty,
            CVPath = application.CV?.FilePath,
            Status = application.Status.ToString(),
            CompatibilityScore = (double?)(application.CompatibilityScore),
            ScoreDetails = application.ScoreDetails,
            CreatedAt = application.AppliedAt,
            UpdatedAt = application.ReviewedAt ?? application.AppliedAt
        };

        if (!string.IsNullOrEmpty(application.ScoreDetails))
    {
        try 
        {
            var details = System.Text.Json.JsonSerializer.Deserialize<EasyApply.BusinessLayer.Structure.DTOs.AI.CompatibilityResultDto>(application.ScoreDetails);
            if (details != null)
            {
                dto.Advantages = details.Advantages;
                dto.Disadvantages = details.Disadvantages;
            }
        }
        catch { /* Fallback */ }
    }
    return dto;
    }
}
