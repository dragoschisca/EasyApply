using System;

namespace EasyApply.Domain.Entities;

public class CompanyReviewHelpful
{
    public Guid ReviewId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public CompanyReview Review { get; set; } = null!;
    public User User { get; set; } = null!;
}
