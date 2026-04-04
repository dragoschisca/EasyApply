using EasyApply.Domain.Models.Enums;

namespace EasyApply.BusinessLayer.Structure.DTOs.Job;

public class CreateJobDto
{
    public Guid CompanyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string? RequiredSkills { get; set; }
    public WorkTime EmploymentType { get; set; }
    public ExperienceLevel ExperienceLevel { get; set; }
    public string? Location { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public bool IsRemote { get; set; }
    public DateTime? ExpiresAt { get; set; }
}