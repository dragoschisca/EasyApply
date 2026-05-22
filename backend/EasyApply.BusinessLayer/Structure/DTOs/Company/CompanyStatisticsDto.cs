using System;
using System.Collections.Generic;

namespace EasyApply.BusinessLayer.Structure.DTOs.Company;

public class CompanyStatisticsDto
{
    public int TotalApplicants { get; set; }
    public List<DailyViewDto> WeeklyProfileViews { get; set; } = new();
    public List<JobPopularityDto> TopJobs { get; set; } = new();
}

public class DailyViewDto
{
    public DateTime Date { get; set; }
    public int Views { get; set; }
}

public class JobPopularityDto
{
    public Guid JobId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ViewsCount { get; set; }
    public int ApplicationsCount { get; set; }
    public double ConversionRate { get; set; }
}
