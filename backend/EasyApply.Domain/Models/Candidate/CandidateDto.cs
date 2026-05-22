using System;

namespace EasyApply.Domain.Models.Candidate;

public class CandidateDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Location { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? Bio { get; set; }
    public string Email { get; set; } = string.Empty;
    public int CVsCount { get; set; }
    public int ApplicationsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}