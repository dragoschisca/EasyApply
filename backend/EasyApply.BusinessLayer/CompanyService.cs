using EasyApply.BusinessLayer.Structure.DTOs.Company;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Exceptions;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.BusinessLayer.Interfaces.Services;

namespace EasyApply.BusinessLayer.Core;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IApplicationRepository _applicationRepository;

    public CompanyService(
        ICompanyRepository companyRepository, 
        IJobRepository jobRepository,
        IApplicationRepository applicationRepository)
    {
        _companyRepository = companyRepository;
        _jobRepository = jobRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<CompanyDto> GetByIdAsync(Guid id)
    {
        var company = await _companyRepository.GetWithDetailsAsync(id);
        if (company == null) throw new NotFoundException($"Company with ID {id} not found.");
        return MapToDto(company);
    }

    public async Task<CompanyDto> GetByUserIdAsync(Guid userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null) throw new NotFoundException("Company profile not found.");
        return MapToDto(company);
    }

    public async Task<List<CompanyDto>> GetAllAsync(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        var (companies, _) = await _companyRepository.GetPagedAsync(skip, pageSize);
        return companies.Select(MapToDto).ToList();
    }

    public async Task<CompanyDto> CreateAsync(Guid userId, CreateCompanyDto dto)
    {
        var existing = await _companyRepository.GetByUserIdAsync(userId);
        if (existing != null) throw new BusinessException("Company profile already exists for this user.");

        var company = new Company
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyName = dto.CompanyName,
            Industry = dto.Industry,
            CompanySize = dto.CompanySize,
            Website = dto.Website,
            Description = dto.Description,
            LogoUrl = dto.LogoUrl,
            Location = dto.Location,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _companyRepository.AddAsync(company);
        await _companyRepository.SaveChangesAsync();

        return MapToDto(company);
    }

    public async Task<CompanyDto> UpdateAsync(Guid userId, UpdateCompanyDto dto)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null) throw new NotFoundException("Company profile not found.");

        if (!string.IsNullOrWhiteSpace(dto.CompanyName)) company.CompanyName = dto.CompanyName;
        if (dto.Industry != null) company.Industry = dto.Industry;
        if (dto.CompanySize != null) company.CompanySize = dto.CompanySize;
        if (dto.Website != null) company.Website = dto.Website;
        if (dto.Description != null) company.Description = dto.Description;
        if (dto.LogoUrl != null) company.LogoUrl = dto.LogoUrl;
        if (dto.Location != null) company.Location = dto.Location;
        if (dto.WhyJoinUs != null) company.WhyJoinUs = dto.WhyJoinUs;
        if (dto.SubscriptionTier.HasValue) company.SubscriptionTier = dto.SubscriptionTier.Value;

        company.UpdatedAt = DateTime.UtcNow;

        await _companyRepository.UpdateAsync(company);
        await _companyRepository.SaveChangesAsync();

        return MapToDto(company);
    }

    public async Task<CompanyStatisticsDto> GetStatisticsAsync(Guid companyId)
    {
        var last7Days = DateTime.UtcNow.Date.AddDays(-6);

        // 1. Total Applicants
        var totalApplicants = await _applicationRepository.GetCountByCompanyIdAsync(companyId);

        // 2. Weekly Profile Views
        var profileViewsRaw = await _companyRepository.GetProfileViewsAsync(companyId, last7Days);
        var viewsByDate = profileViewsRaw
            .GroupBy(v => v.ViewedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToList();

        var weeklyProfileViews = Enumerable.Range(0, 7)
            .Select(offset => last7Days.AddDays(offset))
            .Select(date => new DailyViewDto
            {
                Date = date,
                Views = viewsByDate.FirstOrDefault(p => p.Date == date)?.Count ?? 0
            })
            .ToList();

        // 3. Top 5 Most Popular Jobs
        var rankedJobs = await _jobRepository.GetTopJobsByCompanyIdAsync(companyId, 5);
        var topJobs = rankedJobs.Select(j => new JobPopularityDto
        {
            JobId = j.Id,
            Title = j.Title,
            ViewsCount = j.ViewsCount,
            ApplicationsCount = j.ApplicationsCount,
            ConversionRate = j.ViewsCount > 0 ? (double)j.ApplicationsCount / j.ViewsCount : 0
        }).ToList();

        return new CompanyStatisticsDto
        {
            TotalApplicants = totalApplicants,
            WeeklyProfileViews = weeklyProfileViews,
            TopJobs = topJobs
        };
    }

    public async Task DeleteAsync(Guid userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null) throw new NotFoundException("Company profile not found.");

        await _companyRepository.DeleteAsync(company);
        await _companyRepository.SaveChangesAsync();
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        await _companyRepository.AddProfileViewAsync(new CompanyProfileView
        {
            Id = Guid.NewGuid(),
            CompanyId = id,
            ViewedAt = DateTime.UtcNow
        });
        await _companyRepository.SaveChangesAsync();
    }

    private static CompanyDto MapToDto(Company company)
    {
        return new CompanyDto
        {
            Id = company.Id,
            UserId = company.UserId,
            CompanyName = company.CompanyName,
            Industry = company.Industry,
            CompanySize = company.CompanySize,
            Website = company.Website,
            Description = company.Description,
            LogoUrl = company.LogoUrl,
            Location = company.Location,
            WhyJoinUs = company.WhyJoinUs,
            SubscriptionTier = company.SubscriptionTier,
            SubscriptionExpiresAt = company.SubscriptionExpiresAt,
            CreatedAt = company.CreatedAt
        };
    }
}
