using EasyApply.BusinessLayer.Structure.DTOs.Company;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Exceptions;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.BusinessLayer.Interfaces.Services;

namespace EasyApply.BusinessLayer.Core;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
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

    public async Task DeleteAsync(Guid userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null) throw new NotFoundException("Company profile not found.");

        await _companyRepository.DeleteAsync(company);
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
