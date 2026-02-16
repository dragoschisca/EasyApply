using EasyApply.Core.Enums;

namespace EasyApply.Core.Entites;

public class Job
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string? RequiredSkills { get; set; } // JSON string
    public WorkTime EmploymentType { get; set; }
    public ExperienceLevel ExperienceLevel { get; set; }
    public string? Location { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public bool IsRemote { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int ViewsCount { get; set; } = 0;
    public int ApplicationsCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    public Company Company { get; set; } = null!;
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
}