using System;

namespace EasyApply.BusinessLayer.Structure.DTOs.CompanyReview;

public class ReviewFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } // e.g., "latest", "rating_desc", "rating_asc", "helpful"
    public int? RatingFilter { get; set; } // e.g., filter only 5-star reviews
}
