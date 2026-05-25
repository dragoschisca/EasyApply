using EasyApply.BusinessLayer.Structure.DTOs.Application;
using EasyApply.BusinessLayer.Structure.DTOs.AI;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Enums;
using EasyApply.Domain.Exceptions;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.BusinessLayer.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace EasyApply.BusinessLayer.Core;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IGeminiService _geminiService;
    private readonly ISupabaseStorageService _storageService;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly string _cvBucket;

    public ApplicationService(
        IApplicationRepository applicationRepository, 
        IJobRepository jobRepository, 
        ICandidateRepository candidateRepository,
        IGeminiService geminiService,
        ISupabaseStorageService storageService,
        INotificationService notificationService,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _applicationRepository = applicationRepository;
        _jobRepository = jobRepository;
        _candidateRepository = candidateRepository;
        _geminiService = geminiService;
        _storageService = storageService;
        _notificationService = notificationService;
        _emailService = emailService;
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
        if (exists) throw new ConflictException("You have already applied to this job.");

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
        
        // Increment application count on job (Atomic transaction via shared DbContext)
        job.ApplicationsCount++;
        await _jobRepository.UpdateAsync(job);
        
        await _applicationRepository.SaveChangesAsync();

        // Send Application Received Email
        var candidate = await _candidateRepository.GetWithDetailsAsync(candidateId);
        if (candidate != null && candidate.User != null)
        {
        // Fire-and-forget: email confirmation does not block the response.
        // Failures are logged internally by EmailService.
        _ = _emailService.SendApplicationReceivedEmailAsync(
            candidate.User.Email,
            $"{candidate.FirstName} {candidate.LastName}",
            job.Title);
        }

        var savedApplication = await _applicationRepository.GetWithDetailsAsync(application.Id);
        return MapToDto(savedApplication ?? application);
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

        if (status == ApplicationStatus.Rejected)
        {
            application.RejectionFeedback = dto.Feedback;
        }

        await _applicationRepository.UpdateAsync(application);

        // Add history record
        var history = new ApplicationStatusHistory
        {
            Id = Guid.NewGuid(),
            ApplicationId = application.Id,
            Status = status,
            Feedback = dto.Feedback,
            ChangedBy = "Recruiter",
            ChangedAt = DateTime.UtcNow
        };
        await _applicationRepository.AddStatusHistoryAsync(history);
        
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

            if (status == ApplicationStatus.Rejected && application.Candidate.User != null)
            {
                _ = _emailService.SendApplicationRejectionEmailAsync(
                    application.Candidate.User.Email,
                    $"{application.Candidate.FirstName} {application.Candidate.LastName}",
                    jobTitle,
                    dto.Feedback ?? string.Empty);
            }
        }

        return MapToDto(application);
    }

    public async Task<IEnumerable<StatusChangeDto>> GetStatusTimelineAsync(Guid id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application == null) throw new NotFoundException($"Application with ID {id} not found.");

        var history = await _applicationRepository.GetStatusTimelineAsync(id);
        return history.Select(h => new StatusChangeDto
        {
            Status = h.Status.ToString(),
            ChangedAt = h.ChangedAt,
            Feedback = h.Feedback,
            ChangedBy = h.ChangedBy
        });
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
        application.ScoreDetails = JsonSerializer.Serialize(result);
        
        await _applicationRepository.UpdateAsync(application);
        await _applicationRepository.SaveChangesAsync();

        return MapToDto(application);
    }

    private ApplicationDto MapToDto(Application application)
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
            RejectionFeedback = application.RejectionFeedback,
            CreatedAt = application.AppliedAt,
            UpdatedAt = application.ReviewedAt ?? application.AppliedAt
        };

        if (!string.IsNullOrEmpty(application.ScoreDetails))
        {
            try 
            {
                var details = JsonSerializer.Deserialize<CompatibilityResultDto>(application.ScoreDetails);
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
