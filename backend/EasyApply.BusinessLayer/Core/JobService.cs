using EasyApply.BusinessLayer.Structure.DTOs.Job;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Exceptions;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.BusinessLayer.Interfaces.Services;
using System.Text.Json;

namespace EasyApply.BusinessLayer.Core;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IGeocodingService _geocodingService;

    public JobService(
        IJobRepository jobRepository,
        ICompanyRepository companyRepository,
        IGeocodingService geocodingService)
    {
        _jobRepository = jobRepository;
        _companyRepository = companyRepository;
        _geocodingService = geocodingService;
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
        string? keyword, string? location, string? category, string? employmentType,
        string? experienceLevel, int? locationType, decimal? minSalary, decimal? maxSalary, int page, int pageSize)
    {
        var result = await _jobRepository.SearchAsync(
            keyword, location, category, employmentType, experienceLevel,
            locationType, minSalary, maxSalary, page, pageSize);
        return (result.Jobs.Select(MapToDto), result.Total);
    }

    public async Task<IEnumerable<JobDto>> GetNearbyAsync(double latitude, double longitude, double radiusKm)
    {
        var jobs = await _jobRepository.GetNearbyAsync(latitude, longitude, radiusKm);
        return jobs.Select(MapToDto);
    }

    public async Task<JobDto> CreateAsync(CreateJobDto dto)
    {
        var company = await _companyRepository.GetByIdAsync(dto.CompanyId);
        if (company == null) throw new NotFoundException($"Company with ID {dto.CompanyId} not found.");

        // Geocode the address server-side so coordinates are always stored in the DB.
        double? lat = null, lon = null;
        var addressToGeocode = dto.Address ?? dto.Location;
        if (!string.IsNullOrWhiteSpace(addressToGeocode) &&
            !addressToGeocode.Equals("remote", StringComparison.OrdinalIgnoreCase))
        {
            (lat, lon) = await _geocodingService.GeocodeAsync(addressToGeocode);
        }

        var job = new Job
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            Title = dto.Title,
            Category = dto.Category,
            Description = dto.Description,
            Requirements = dto.Requirements,
            RequiredSkills = dto.RequiredSkills,
            EmploymentType = dto.EmploymentType,
            ExperienceLevel = dto.ExperienceLevel,
            LocationType = dto.LocationType,
            Location = dto.Location,
            Address = dto.Address,
            CompanyCulture = dto.CompanyCulture,
            Latitude = lat,
            Longitude = lon,
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

        // Re-attach company for the DTO mapping (navigation property not populated after Add).
        job.Company = company;
        return MapToDto(job);
    }

    public async Task<JobDto> UpdateAsync(Guid id, UpdateJobDto dto)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null) throw new NotFoundException($"Job with ID {id} not found.");

        if (!string.IsNullOrWhiteSpace(dto.Title)) job.Title = dto.Title;
        if (!string.IsNullOrWhiteSpace(dto.Category)) job.Category = dto.Category;
        if (!string.IsNullOrWhiteSpace(dto.Description)) job.Description = dto.Description;
        if (!string.IsNullOrWhiteSpace(dto.Requirements)) job.Requirements = dto.Requirements;
        if (dto.RequiredSkills != null) job.RequiredSkills = dto.RequiredSkills;
        if (dto.EmploymentType.HasValue) job.EmploymentType = dto.EmploymentType.Value;
        if (dto.ExperienceLevel.HasValue) job.ExperienceLevel = dto.ExperienceLevel.Value;
        if (dto.LocationType.HasValue) job.LocationType = dto.LocationType.Value;
        if (dto.Location != null) job.Location = dto.Location;
        if (dto.CompanyCulture != null) job.CompanyCulture = dto.CompanyCulture;
        if (dto.SalaryMin.HasValue) job.SalaryMin = dto.SalaryMin.Value;
        if (dto.SalaryMax.HasValue) job.SalaryMax = dto.SalaryMax.Value;
        if (dto.IsRemote.HasValue) job.IsRemote = dto.IsRemote.Value;
        if (dto.IsActive.HasValue) job.IsActive = dto.IsActive.Value;
        if (dto.ExpiresAt.HasValue) job.ExpiresAt = dto.ExpiresAt.Value;

        // Re-geocode if the address changed.
        if (dto.Address != null && dto.Address != job.Address)
        {
            job.Address = dto.Address;
            var addressToGeocode = dto.Address.Length > 0 ? dto.Address : job.Location;
            if (!string.IsNullOrWhiteSpace(addressToGeocode) &&
                !addressToGeocode.Equals("remote", StringComparison.OrdinalIgnoreCase))
            {
                (job.Latitude, job.Longitude) = await _geocodingService.GeocodeAsync(addressToGeocode);
            }
            else
            {
                job.Latitude = null;
                job.Longitude = null;
            }
        }

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

    public async Task<IEnumerable<JobDto>> GetRecommendationsAsync(Guid id, int count)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null) return Enumerable.Empty<JobDto>();

        var allJobs = await _jobRepository.GetAllAsync();
        var recommendations = allJobs
            .Where(j => j.Id != id && j.IsActive && j.CompanyId == job.CompanyId)
            .Take(count);

        return recommendations.Select(MapToDto);
    }

    private static JobDto MapToDto(Job job)
    {
        var dto = new JobDto
        {
            Id = job.Id,
            CompanyId = job.CompanyId,
            CompanyName = job.Company?.CompanyName ?? string.Empty,
            CompanyLogoUrl = job.Company?.LogoUrl,
            Title = job.Title,
            Category = job.Category,
            Description = job.Description,
            Requirements = job.Requirements,
            EmploymentType = job.EmploymentType,
            ExperienceLevel = job.ExperienceLevel,
            LocationType = job.LocationType,
            Location = job.Location,
            Address = job.Address,
            CompanyCulture = job.CompanyCulture,
            Latitude = job.Latitude,
            Longitude = job.Longitude,
            SalaryMin = job.SalaryMin,
            SalaryMax = job.SalaryMax,
            IsRemote = job.IsRemote,
            IsActive = job.IsActive,
            ViewsCount = job.ViewsCount,
            ApplicationsCount = job.ApplicationsCount,
            CreatedAt = job.CreatedAt,
            ExpiresAt = job.ExpiresAt,
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