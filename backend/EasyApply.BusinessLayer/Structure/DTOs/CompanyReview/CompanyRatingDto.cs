using System;

namespace EasyApply.BusinessLayer.Structure.DTOs.CompanyReview;

public class CompanyRatingDto
{
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public string RatingDistribution { get; set; } = "{\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0}";
    public DateTime LastUpdated { get; set; }
}
