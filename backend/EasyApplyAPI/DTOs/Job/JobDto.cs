using System;
using System.Collections.Generic;

namespace EasyApplyAPI.DTOs.Job;

public class JobDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyLogoUrl { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public List<string> RequiredSkills { get; set; } = new();
    public WorkTime EmploymentType { get; set; }
    public ExperienceLevel ExperienceLevel { get; set; }
    public string? Location { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public bool IsRemote { get; set; }
    public bool IsActive { get; set; }
    public int ViewsCount { get; set; }
    public int ApplicationsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}