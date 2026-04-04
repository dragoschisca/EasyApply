using EasyApply.BusinessLayer.Structure.DTOs.Job;
using EasyApply.BusinessLayer.Interfaces.Services;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Models.Interfaces.Repositories;
using System.Text.Json;

namespace EasyApply.BusinessLayer.Core;

public class RecommendationService : IRecommendationService
{
    private readonly IJobRepository _jobRepository;
    private readonly ICandidateRepository _candidateRepository;

    public RecommendationService(IJobRepository jobRepository, ICandidateRepository candidateRepository)
    {
        _jobRepository = jobRepository;
        _candidateRepository = candidateRepository;
    }

    public async Task<IEnumerable<JobDto>> GetRecommendedJobsAsync(Guid userId)
    {
        var candidate = await _candidateRepository.GetByUserIdAsync(userId);
        if (candidate == null) return Enumerable.Empty<JobDto>();

        // Get primary CV skills
        var skills = new List<string>();
        var primaryCv = candidate.CVs.FirstOrDefault(c => c.IsPrimary) ?? candidate.CVs.FirstOrDefault();
        if (primaryCv != null && !string.IsNullOrEmpty(primaryCv.Skills))
        {
            try {
                skills = JsonSerializer.Deserialize<List<string>>(primaryCv.Skills) ?? new List<string>();
            } catch { /* ignored */ }
        }

        var allJobs = await _jobRepository.GetAllAsync();
        var activeJobs = allJobs.Where(j => j.IsActive).ToList();

        var recommendations = activeJobs
            .Select(job => new { 
                Job = job, 
                Score = CalculateMatchScore(job, candidate, skills) 
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(5)
            .Select(x => MapToDto(x.Job))
            .ToList();

        return recommendations;
    }

    private static int CalculateMatchScore(Job job, Candidate candidate, List<string> skills)
    {
        int score = 0;

        // Location Match
        if (!string.IsNullOrEmpty(job.Location) && job.Location.Equals(candidate.Location, StringComparison.OrdinalIgnoreCase))
        {
            score += 30;
        }

        // Skill Match (Category vs Skills)
        if (!string.IsNullOrEmpty(job.Category) && skills.Any(s => s.Contains(job.Category, StringComparison.OrdinalIgnoreCase)))
        {
            score += 40;
        }

        // Partial Title Match
        if (skills.Any(s => job.Title.Contains(s, StringComparison.OrdinalIgnoreCase)))
        {
            score += 20;
        }

        return score;
    }

    private static JobDto MapToDto(Job job)
    {
        var dto = new JobDto
        {
            Id = job.Id,
            CompanyId = job.CompanyId,
            Title = job.Title,
            Description = job.Description,
            Requirements = job.Requirements,
            EmploymentType = job.EmploymentType,
            ExperienceLevel = job.ExperienceLevel,
            Location = job.Location,
            Address = job.Address,
            SalaryMin = job.SalaryMin,
            SalaryMax = job.SalaryMax,
            Latitude = job.Latitude,
            Longitude = job.Longitude,
            LocationType = job.LocationType,
            CompanyCulture = job.CompanyCulture,
            IsActive = job.IsActive,
            ViewsCount = job.ViewsCount,
            ApplicationsCount = job.ApplicationsCount,
            CreatedAt = job.CreatedAt,
            ExpiresAt = job.ExpiresAt,
            CompanyName = job.Company?.CompanyName ?? string.Empty,
            CompanyLogoUrl = job.Company?.LogoUrl
        };

        if (!string.IsNullOrEmpty(job.RequiredSkills))
        {
            try
            {
                dto.RequiredSkills = JsonSerializer.Deserialize<List<string>>(job.RequiredSkills) ?? new List<string>();
            }
            catch
            {
                // ignored
            }
        }

        return dto;
    }
}
