using System;

namespace EasyApply.Domain.Entities;

public class CompanyRating
{
    public Guid CompanyId { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public string RatingDistribution { get; set; } = "{\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0}"; // JSON dictionary mapping stars (1-5) to counts
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Company Company { get; set; } = null!;
}
