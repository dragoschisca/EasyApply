namespace EasyApply.Domains.Enums;

public class Candidate
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Location { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<CV> CVs { get; set; } = new List<CV>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
    public string FullName => $"{FirstName} {LastName}";
}