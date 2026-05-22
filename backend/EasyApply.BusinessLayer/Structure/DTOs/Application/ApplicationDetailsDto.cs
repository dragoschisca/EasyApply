namespace EasyApply.BusinessLayer.Structure.DTOs.Application;

public class ApplicationDetailsDto : ApplicationDto
{
    public string CandidateFirstName { get; set; } = string.Empty;
    public string CandidateLastName { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public string? CompatibilityDetails { get; set; }
    public string? RecruiterNotes { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
