using System;
using EasyApply.Domain.Enums;

namespace EasyApply.BusinessLayer.Structure.DTOs.CompanyReview;

public class UpdateCompanyReviewDto
{
    public int Rating { get; set; } // 1-5
    public string Title { get; set; } = string.Empty;
    public string ReviewText { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public InterviewExperience InterviewExperience { get; set; } = InterviewExperience.None;
    public decimal? SalaryOffered { get; set; }
    public int? HiringProcessDuration { get; set; } // in days
}
