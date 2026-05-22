using EasyApply.BusinessLayer.Structure.DTOs.Job;
using EasyApply.BusinessLayer.Structure.Validation;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Exceptions;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.BusinessLayer.Interfaces.Services;
using EasyApply.Domain.Models.Job;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace EasyApply.BusinessLayer.Core;

public class JobService : IJobService
{
    private const decimal DefaultMarketSalary = 25000;
    private readonly IJobRepository _jobRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IGeocodingService _geocodingService;
    private readonly ILogger<JobService> _logger;

    public JobService(
        IJobRepository jobRepository,
        ICompanyRepository companyRepository,
        IGeocodingService geocodingService,
        ILogger<JobService> logger)
    {
        _jobRepository = jobRepository;
        _companyRepository = companyRepository;
        _geocodingService = geocodingService;
        _logger = logger;
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


    public async Task<SearchJobResultDto> SearchAsync(SearchJobDto searchDto)
    {
        if (searchDto.Page < 1)
        {
            throw new ValidationException(new Dictionary<string, string[]> { { nameof(searchDto.Page), new[] { "Page must be greater than or equal to 1." } } });
        }
        if (searchDto.PageSize < 1 || searchDto.PageSize > 100)
        {
            throw new ValidationException(new Dictionary<string, string[]> { { nameof(searchDto.PageSize), new[] { "PageSize must be between 1 and 100." } } });
        }

        var (items, totalCount) = await _jobRepository.SearchAsync(searchDto);

        var jobs = items.Select(MapToDto).ToList();
        var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

        return new SearchJobResultDto
        {
            Jobs = jobs,
            TotalCount = totalCount,
            Page = searchDto.Page,
            PageSize = searchDto.PageSize,
            TotalPages = totalPages
        };
    }

    public async Task<IEnumerable<JobDto>> GetNearbyAsync(double latitude, double longitude, double radiusKm)
    {
        var jobs = await _jobRepository.GetNearbyAsync(latitude, longitude, radiusKm);
        return jobs.Select(MapToDto);
    }

    public async Task<JobDto> CreateAsync(CreateJobDto dto)
    {
        ValidationHelper.ValidateCreateJob(dto);
        var company = await _companyRepository.GetByIdAsync(dto.CompanyId);
        if (company == null) throw new NotFoundException($"Company with ID {dto.CompanyId} not found.");

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

        job.Company = company;
        return MapToDto(job);
    }

    public async Task<JobDto> UpdateAsync(Guid id, UpdateJobDto dto)
    {
        ValidationHelper.ValidateUpdateJob(dto);
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
        if (dto.SalaryMin.HasValue) job.SalaryMin = dto.SalaryMin.Value;
        if (dto.SalaryMax.HasValue) job.SalaryMax = dto.SalaryMax.Value;
        if (dto.IsRemote.HasValue) job.IsRemote = dto.IsRemote.Value;
        if (dto.IsActive.HasValue) job.IsActive = dto.IsActive.Value;
        if (dto.ExpiresAt.HasValue) job.ExpiresAt = dto.ExpiresAt.Value;

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
        await _jobRepository.IncrementViewCountAsync(id, saveChanges: false);
        
        await _jobRepository.AddJobViewAsync(new JobView
        {
            Id = Guid.NewGuid(),
            JobId = id,
            ViewedAt = DateTime.UtcNow
        });
        
        await _jobRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<JobDto>> GetRecommendationsAsync(Guid id, int count)
    {
        var recommendations = await _jobRepository.GetRecommendationsAsync(id, count);
        return recommendations.Select(MapToDto);
    }

    public async Task<SalaryBenchmarkResponse> GetSalaryBenchmarkAsync(SalaryBenchmarkRequest request)
    {
        var benchmarkData = await _jobRepository.GetSalaryBenchmarkDataAsync(request.Category, request.ExperienceLevel);
        
        var averages = benchmarkData
            .Select(d => (d.Min ?? d.Max ?? 0) + (d.Max ?? d.Min ?? 0))
            .Select(sum => sum / 2)
            .ToList();

        decimal marketAverage = averages.Any() ? averages.Average() : 0;
        
        if (marketAverage == 0) marketAverage = DefaultMarketSalary;

        decimal targetSalary = ((request.SalaryMin ?? request.SalaryMax ?? 0) + (request.SalaryMax ?? request.SalaryMin ?? 0)) / 2;
        
        if (targetSalary == 0)
        {
            return new SalaryBenchmarkResponse
            {
                MarketAverage = Math.Round(marketAverage, 0),
                PercentageDifference = 0,
                StatusLabel = "Negotiable"
            };
        }

        double percentageDiff = (double)((targetSalary - marketAverage) / marketAverage) * 100;
        
        string statusLabel = "Fair Market Value";
        if (percentageDiff > 15) statusLabel = "Highly Competitive";
        else if (percentageDiff > 5) statusLabel = "Competitive";
        else if (percentageDiff < -15) statusLabel = "Below Average";
        else if (percentageDiff < -5) statusLabel = "Slightly Below Market";

        return new SalaryBenchmarkResponse
        {
            MarketAverage = Math.Round(marketAverage, 0),
            PercentageDifference = Math.Round(percentageDiff, 1),
            StatusLabel = statusLabel
        };
    }

    private JobDto MapToDto(Job job)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize RequiredSkills for job {JobId}", job.Id);
            }
        }

        return dto;
    }
}