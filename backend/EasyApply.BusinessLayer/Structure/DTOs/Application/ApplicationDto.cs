namespace EasyApply.BusinessLayer.Structure.DTOs.Application;

public class ApplicationDto
{
    public Guid Id { get; set; }
    public Guid CandidateId { get; set; }
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CVFileName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double? CompatibilityScore { get; set; }
    public string? CoverLetter { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}