namespace EasyApply.BusinessLayer.Structure.DTOs.Application;

public class ApplicationDto
{
    public Guid Id { get; set; }
    public Guid CandidateId { get; set; }
    public Guid JobId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CVFileName { get; set; } = string.Empty;
    public string? CVPath { get; set; }
    public string Status { get; set; } = string.Empty;
    public double? CompatibilityScore { get; set; }
    public List<string> Advantages { get; set; } = new();
    public List<string> Disadvantages { get; set; } = new();
    public string? ScoreDetails { get; set; }
    public string? CoverLetter { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}