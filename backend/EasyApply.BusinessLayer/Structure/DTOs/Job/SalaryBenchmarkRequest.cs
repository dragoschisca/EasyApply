namespace EasyApply.BusinessLayer.Structure.DTOs.Job;

public class SalaryBenchmarkRequest
{
    public string Category { get; set; } = string.Empty;
    public string ExperienceLevel { get; set; } = string.Empty;
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
}


