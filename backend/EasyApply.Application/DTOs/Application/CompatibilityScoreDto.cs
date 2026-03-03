namespace EasyApply.Application.DTOs.Application;

public class CompatibilityScoreDto
{
    public double Score { get; set; }
    public string? Details { get; set; }
    public List<string> MatchingSkills { get; set; } = new();
    public List<string> MissingSkills { get; set; } = new();
}
