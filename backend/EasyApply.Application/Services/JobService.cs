using EasyApply.Application.DTOs.Job;
using EasyApply.Domains.Entities;
using EasyApply.Domains.Exceptions;
using EasyApply.Domains.Interfaces.Repositories;
using EasyApply.Application.Interfaces.Services;
using System.Text.Json;

namespace EasyApply.Application.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly ICompanyRepository _companyRepository;

    public JobService(IJobRepository jobRepository, ICompanyRepository companyRepository)
    {
        _jobRepository = jobRepository;
        _companyRepository = companyRepository;
    }

    public async Task<JobDto> GetByIdAsync(Guid id)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null) throw new NotFoundException($"Job with ID {id} not found.");
        return MapToDto(job);
    }

    public async Task<IEnumerable<JobDto>> GetByCompanyIdAsync(Guid companyId, bool activeOnly = true)
    {
        var jobs = await _jobRepository.GetByCompanyIdAsync(companyId, activeOnly);
        return jobs.Select(MapToDto);
    }

    public async Task<(IEnumerable<JobDto> Jobs, int Total)> SearchAsync(
        string? keyword, string? location, string? employmentType,
        string? experienceLevel, bool? isRemote, int page, int pageSize)
    {
        var result = await _jobRepository.SearchAsync(keyword, location, employmentType, experienceLevel, isRemote, page, pageSize);
        return (result.Jobs.Select(MapToDto), result.Total);
    }

    public async Task<JobDto> CreateAsync(CreateJobDto dto)
    {
        var company = await _companyRepository.GetByIdAsync(dto.CompanyId);
        if (company == null) throw new NotFoundException($"Company with ID {dto.CompanyId} not found.");

        var job = new Job
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            Title = dto.Title,
            Description = dto.Description,
            Requirements = dto.Requirements,
            RequiredSkills = dto.RequiredSkills,
            EmploymentType = dto.EmploymentType,
            ExperienceLevel = dto.ExperienceLevel,
            Location = dto.Location,
            SalaryMin = dto.SalaryMin,
            SalaryMax = dto.SalaryMax,
            IsRemote = dto.IsRemote,
            IsActive = true,
            ViewsCount = 0,
            ApplicationsCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ExpiresAt = dto.ExpiresAt
        };

        await _jobRepository.AddAsync(job);
        await _jobRepository.SaveChangesAsync();

        return MapToDto(job);
    }

    public async Task<JobDto> UpdateAsync(Guid id, UpdateJobDto dto)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null) throw new NotFoundException($"Job with ID {id} not found.");

        if (!string.IsNullOrWhiteSpace(dto.Title)) job.Title = dto.Title;
        if (!string.IsNullOrWhiteSpace(dto.Description)) job.Description = dto.Description;
        if (!string.IsNullOrWhiteSpace(dto.Requirements)) job.Requirements = dto.Requirements;
        if (dto.RequiredSkills != null) job.RequiredSkills = dto.RequiredSkills;
        if (dto.EmploymentType.HasValue) job.EmploymentType = dto.EmploymentType.Value;
        if (dto.ExperienceLevel.HasValue) job.ExperienceLevel = dto.ExperienceLevel.Value;
        if (dto.Location != null) job.Location = dto.Location;
        if (dto.SalaryMin.HasValue) job.SalaryMin = dto.SalaryMin.Value;
        if (dto.SalaryMax.HasValue) job.SalaryMax = dto.SalaryMax.Value;
        if (dto.IsRemote.HasValue) job.IsRemote = dto.IsRemote.Value;
        if (dto.IsActive.HasValue) job.IsActive = dto.IsActive.Value;
        if (dto.ExpiresAt.HasValue) job.ExpiresAt = dto.ExpiresAt.Value;

        job.UpdatedAt = DateTime.UtcNow;

        await _jobRepository.UpdateAsync(job);
        await _jobRepository.SaveChangesAsync();

        return MapToDto(job);
    }

    public async Task DeleteAsync(Guid id)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null) throw new NotFoundException($"Job with ID {id} not found.");

        await _jobRepository.DeleteAsync(job);
        await _jobRepository.SaveChangesAsync();
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        await _jobRepository.IncrementViewCountAsync(id);
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
            SalaryMin = job.SalaryMin,
            SalaryMax = job.SalaryMax,
            IsRemote = job.IsRemote,
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
