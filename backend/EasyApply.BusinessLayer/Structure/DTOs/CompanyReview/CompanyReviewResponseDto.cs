using System;
using EasyApply.Domain.Enums;

namespace EasyApply.BusinessLayer.Structure.DTOs.CompanyReview;

public class CompanyReviewResponseDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid UserId { get; set; }
    public string AuthorName { get; set; } = "Anonymous Candidate";
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ReviewText { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public InterviewExperience InterviewExperience { get; set; }
    public decimal? SalaryOffered { get; set; }
    public int? HiringProcessDuration { get; set; }
    public string? CompanyResponse { get; set; }
    public int HelpfulCount { get; set; }
    public bool IsVerified { get; set; }
    public bool HasUpvoted { get; set; } // If the currently logged in user upvoted this review
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
