using EasyApply.Domain.Enums;

namespace EasyApply.Domain.Models.Job;

public enum JobSortOption
{
    Relevance,
    Newest,
    MostApplied
}

public class SearchJobDto
{
    public string? SearchTerm { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string? LocationFilter { get; set; }
    public WorkTime? EmploymentType { get; set; }
    public ExperienceLevel? ExperienceLevel { get; set; }
    public List<string>? Skills { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public JobSortOption SortBy { get; set; } = JobSortOption.Relevance;
}
