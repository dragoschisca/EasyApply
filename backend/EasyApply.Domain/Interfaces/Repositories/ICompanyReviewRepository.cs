using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyApply.Domain.Entities;

namespace EasyApply.Domain.Interfaces.Repositories;

public interface ICompanyReviewRepository : IBaseRepository<CompanyReview>
{
    Task<(IEnumerable<CompanyReview> Items, int TotalCount)> GetCompanyReviewsAsync(Guid companyId, int page, int size, string? sortBy, int? ratingFilter);
    Task<IEnumerable<CompanyReview>> GetUserReviewsAsync(Guid userId);
    Task<CompanyReview?> GetUserReviewForCompanyAsync(Guid userId, Guid companyId);
    Task AddHelpfulAsync(CompanyReviewHelpful helpful);
    Task RemoveHelpfulAsync(Guid reviewId, Guid userId);
    Task<bool> HasUserUpvotedAsync(Guid reviewId, Guid userId);
    Task<HashSet<Guid>> GetUpvotedReviewIdsAsync(IEnumerable<Guid> reviewIds, Guid userId);
}
