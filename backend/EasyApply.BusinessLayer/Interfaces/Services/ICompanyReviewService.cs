using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyApply.BusinessLayer.Structure.DTOs.CompanyReview;

namespace EasyApply.BusinessLayer.Interfaces.Services;

public interface ICompanyReviewService
{
    Task<CompanyReviewResponseDto> CreateOrUpdateReviewAsync(Guid userId, Guid companyId, CreateCompanyReviewDto dto);
    Task<CompanyReviewResponseDto> UpdateReviewAsync(Guid userId, Guid reviewId, UpdateCompanyReviewDto dto);
    Task DeleteReviewAsync(Guid userId, Guid reviewId, bool isAdmin);
    Task<(IEnumerable<CompanyReviewResponseDto> Items, int TotalCount)> GetCompanyReviewsAsync(Guid userId, Guid companyId, ReviewFilterDto filter);
    Task ToggleHelpfulAsync(Guid userId, Guid reviewId);
    Task<CompanyRatingDto?> GetCompanyRatingAsync(Guid companyId);
    Task<IEnumerable<CompanyRatingDto>> GetTopRatedCompaniesAsync(int limit);
    Task SubmitCompanyResponseAsync(Guid companyUserId, Guid reviewId, string responseText);
}
