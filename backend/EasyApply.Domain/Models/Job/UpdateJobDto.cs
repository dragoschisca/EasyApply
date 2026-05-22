using EasyApply.Domain.Enums;

namespace EasyApply.Domain.Models.Job;

public class UpdateJobDto
{
    public string? Title { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public string? RequiredSkills { get; set; }
    public WorkTime? EmploymentType { get; set; }
    public ExperienceLevel? ExperienceLevel { get; set; }
    public string? Location { get; set; }
    public string? Address { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public LocationType? LocationType { get; set; }
    public string? CompanyCulture { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
