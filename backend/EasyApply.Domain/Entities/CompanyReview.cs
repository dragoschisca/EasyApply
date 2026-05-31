using System;
using EasyApply.Domain.Enums;

namespace EasyApply.Domain.Entities;

public class CompanyReview
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid UserId { get; set; } // Candidate's User Id
    public int Rating { get; set; } // 1-5 stars
    public string Title { get; set; } = string.Empty;
    public string ReviewText { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public InterviewExperience InterviewExperience { get; set; } = InterviewExperience.None;
    public decimal? SalaryOffered { get; set; }
    public int? HiringProcessDuration { get; set; } // In days
    public string? CompanyResponse { get; set; }
    public int HelpfulCount { get; set; }
    public bool IsVerified { get; set; } // True if user has Completed/Rejected application
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; } // Soft delete

    // Navigation properties
    public Company Company { get; set; } = null!;
    public User User { get; set; } = null!;
}
