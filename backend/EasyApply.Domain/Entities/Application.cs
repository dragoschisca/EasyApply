using EasyApply.Domain.Models.Enums;
namespace EasyApply.Domain.Entities;

public class Application
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid CandidateId { get; set; }
    public Guid CVId { get; set; }
    public decimal? CompatibilityScore { get; set; }
    public string? ScoreDetails { get; set; } // JSON string
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }

    // Navigation 
    public Job Job { get; set; } = null!;
    public Candidate Candidate { get; set; } = null!;
    public CV CV { get; set; } = null!;
}