using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EasyApply.BusinessLayer.Interfaces.Services;
using EasyApply.BusinessLayer.Structure.DTOs.CompanyReview;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Enums;
using EasyApply.Domain.Exceptions;
using EasyApply.Domain.Interfaces.Repositories;

namespace EasyApply.BusinessLayer.Core;

public class CompanyReviewService : ICompanyReviewService
{
    private readonly ICompanyReviewRepository _companyReviewRepository;
    private readonly ICompanyRatingRepository _companyRatingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly ICandidateRepository _candidateRepository;

    public CompanyReviewService(
        ICompanyReviewRepository companyReviewRepository,
        ICompanyRatingRepository companyRatingRepository,
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IApplicationRepository applicationRepository,
        ICandidateRepository candidateRepository)
    {
        _companyReviewRepository = companyReviewRepository;
        _companyRatingRepository = companyRatingRepository;
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _applicationRepository = applicationRepository;
        _candidateRepository = candidateRepository;
    }

    public async Task<CompanyReviewResponseDto> CreateOrUpdateReviewAsync(Guid userId, Guid companyId, CreateCompanyReviewDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found.");

        if (user.UserType == UserType.Company)
        {
            throw new ForbiddenException("Companies are not allowed to submit reviews.");
        }

        var company = await _companyRepository.GetByIdAsync(companyId);
        if (company == null) throw new NotFoundException($"Company with ID {companyId} not found.");

        // Prevent self-review (if the candidate is linked to the company or user is owner of the company profile)
        if (company.UserId == userId)
        {
            throw new ForbiddenException("You cannot review your own company profile.");
        }

        // Retrieve candidate profile to check applications
        var candidate = await _candidateRepository.GetByUserIdAsync(userId);
        var hasCompletedOrRejectedApp = false;
        if (candidate != null)
        {
            var apps = await _applicationRepository.GetByCandidateIdAsync(candidate.Id);
            hasCompletedOrRejectedApp = apps.Any(a => 
                a.Job.CompanyId == companyId && 
                (a.Status == ApplicationStatus.Accepted || a.Status == ApplicationStatus.Rejected));
        }

        // Check if user already reviewed this company
        var existingReview = await _companyReviewRepository.GetUserReviewForCompanyAsync(userId, companyId);
        if (existingReview != null)
        {
            // Edit existing review
            if (existingReview.CreatedAt.AddDays(30) < DateTime.UtcNow)
            {
                throw new BusinessException("Reviews can only be edited within 30 days of submission.");
            }

            existingReview.Rating = dto.Rating;
            existingReview.Title = dto.Title;
            existingReview.ReviewText = dto.ReviewText;
            existingReview.JobTitle = dto.JobTitle;
            existingReview.InterviewExperience = dto.InterviewExperience;
            existingReview.SalaryOffered = dto.SalaryOffered;
            existingReview.HiringProcessDuration = dto.HiringProcessDuration;
            existingReview.IsVerified = hasCompletedOrRejectedApp;
            existingReview.UpdatedAt = DateTime.UtcNow;

            await _companyReviewRepository.UpdateAsync(existingReview);
            await _companyReviewRepository.SaveChangesAsync();

            await RecalculateRatingAsync(companyId);

            return await MapToDtoAsync(existingReview, userId);
        }

        // Create new review
        var review = new CompanyReview
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            UserId = userId,
            Rating = dto.Rating,
            Title = dto.Title,
            ReviewText = dto.ReviewText,
            JobTitle = dto.JobTitle,
            InterviewExperience = dto.InterviewExperience,
            SalaryOffered = dto.SalaryOffered,
            HiringProcessDuration = dto.HiringProcessDuration,
            IsVerified = hasCompletedOrRejectedApp,
            CreatedAt = DateTime.UtcNow
        };

        await _companyReviewRepository.AddAsync(review);
        await _companyReviewRepository.SaveChangesAsync();

        await RecalculateRatingAsync(companyId);

        return await MapToDtoAsync(review, userId);
    }

    public async Task<CompanyReviewResponseDto> UpdateReviewAsync(Guid userId, Guid reviewId, UpdateCompanyReviewDto dto)
    {
        var review = await _companyReviewRepository.GetByIdAsync(reviewId);
        if (review == null) throw new NotFoundException($"Review with ID {reviewId} not found.");

        if (review.UserId != userId)
        {
            throw new ForbiddenException("You can only edit your own reviews.");
        }

        if (review.CreatedAt.AddDays(30) < DateTime.UtcNow)
        {
            throw new BusinessException("Reviews can only be edited within 30 days of submission.");
        }

        review.Rating = dto.Rating;
        review.Title = dto.Title;
        review.ReviewText = dto.ReviewText;
        review.JobTitle = dto.JobTitle;
        review.InterviewExperience = dto.InterviewExperience;
        review.SalaryOffered = dto.SalaryOffered;
        review.HiringProcessDuration = dto.HiringProcessDuration;
        review.UpdatedAt = DateTime.UtcNow;

        await _companyReviewRepository.UpdateAsync(review);
        await _companyReviewRepository.SaveChangesAsync();

        await RecalculateRatingAsync(review.CompanyId);

        return await MapToDtoAsync(review, userId);
    }

    public async Task DeleteReviewAsync(Guid userId, Guid reviewId, bool isAdmin)
    {
        var review = await _companyReviewRepository.GetByIdAsync(reviewId);
        if (review == null) throw new NotFoundException($"Review with ID {reviewId} not found.");

        if (review.UserId != userId && !isAdmin)
        {
            throw new ForbiddenException("You are not authorized to delete this review.");
        }

        await _companyReviewRepository.DeleteAsync(review);
        await _companyReviewRepository.SaveChangesAsync();

        await RecalculateRatingAsync(review.CompanyId);
    }

    public async Task<(IEnumerable<CompanyReviewResponseDto> Items, int TotalCount)> GetCompanyReviewsAsync(
        Guid userId, Guid companyId, ReviewFilterDto filter)
    {
        var (reviews, total) = await _companyReviewRepository.GetCompanyReviewsAsync(
            companyId, filter.PageNumber, filter.PageSize, filter.SortBy, filter.RatingFilter);

        var reviewList = reviews.ToList();
        var upvotedIds = new HashSet<Guid>();
        if (userId != Guid.Empty && reviewList.Any())
        {
            upvotedIds = await _companyReviewRepository.GetUpvotedReviewIdsAsync(reviewList.Select(r => r.Id), userId);
        }

        var list = new List<CompanyReviewResponseDto>();
        foreach (var r in reviewList)
        {
            list.Add(MapToDto(r, upvotedIds.Contains(r.Id)));
        }

        return (list, total);
    }

    public async Task ToggleHelpfulAsync(Guid userId, Guid reviewId)
    {
        var review = await _companyReviewRepository.GetByIdAsync(reviewId);
        if (review == null) throw new NotFoundException($"Review with ID {reviewId} not found.");

        if (review.UserId == userId)
        {
            throw new ForbiddenException("You cannot mark your own review as helpful.");
        }

        var alreadyUpvoted = await _companyReviewRepository.HasUserUpvotedAsync(reviewId, userId);
        if (alreadyUpvoted)
        {
            await _companyReviewRepository.RemoveHelpfulAsync(reviewId, userId);
            review.HelpfulCount = Math.Max(0, review.HelpfulCount - 1);
        }
        else
        {
            var helpful = new CompanyReviewHelpful
            {
                ReviewId = reviewId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            await _companyReviewRepository.AddHelpfulAsync(helpful);
            review.HelpfulCount++;
        }

        await _companyReviewRepository.UpdateAsync(review);
        await _companyReviewRepository.SaveChangesAsync();
    }

    public async Task<CompanyRatingDto?> GetCompanyRatingAsync(Guid companyId)
    {
        var rating = await _companyRatingRepository.GetCompanyRatingAsync(companyId);
        if (rating == null) return null;

        var company = await _companyRepository.GetByIdAsync(companyId);
        return new CompanyRatingDto
        {
            CompanyId = rating.CompanyId,
            CompanyName = company?.CompanyName ?? "Unknown Company",
            AverageRating = rating.AverageRating,
            TotalReviews = rating.TotalReviews,
            RatingDistribution = rating.RatingDistribution,
            LastUpdated = rating.LastUpdated
        };
    }

    public async Task<IEnumerable<CompanyRatingDto>> GetTopRatedCompaniesAsync(int limit)
    {
        var ratings = await _companyRatingRepository.GetTopRatedCompaniesAsync(limit);
        return ratings.Select(r => new CompanyRatingDto
        {
            CompanyId = r.CompanyId,
            CompanyName = r.Company?.CompanyName ?? "Unknown Company",
            AverageRating = r.AverageRating,
            TotalReviews = r.TotalReviews,
            RatingDistribution = r.RatingDistribution,
            LastUpdated = r.LastUpdated
        }).ToList();
    }

    public async Task SubmitCompanyResponseAsync(Guid companyUserId, Guid reviewId, string responseText)
    {
        var review = await _companyReviewRepository.GetByIdAsync(reviewId);
        if (review == null) throw new NotFoundException($"Review with ID {reviewId} not found.");

        var company = await _companyRepository.GetByIdAsync(review.CompanyId);
        if (company == null || company.UserId != companyUserId)
        {
            throw new ForbiddenException("Only the reviewed company can submit a response.");
        }

        review.CompanyResponse = responseText;
        review.UpdatedAt = DateTime.UtcNow;

        await _companyReviewRepository.UpdateAsync(review);
        await _companyReviewRepository.SaveChangesAsync();
    }

    private async Task RecalculateRatingAsync(Guid companyId)
    {
        // Fetch all current active reviews for the company
        var (reviews, totalCount) = await _companyReviewRepository.GetCompanyReviewsAsync(companyId, 1, int.MaxValue, null, null);

        var avg = totalCount > 0 ? (decimal)reviews.Average(r => r.Rating) : 0;

        var distribution = new Dictionary<string, int>
        {
            { "1", 0 },
            { "2", 0 },
            { "3", 0 },
            { "4", 0 },
            { "5", 0 }
        };

        foreach (var r in reviews)
        {
            var key = r.Rating.ToString();
            if (distribution.ContainsKey(key))
            {
                distribution[key]++;
            }
        }

        var rating = new CompanyRating
        {
            CompanyId = companyId,
            AverageRating = Math.Round(avg, 2),
            TotalReviews = totalCount,
            RatingDistribution = JsonSerializer.Serialize(distribution),
            LastUpdated = DateTime.UtcNow
        };

        await _companyRatingRepository.UpsertRatingAsync(rating);
        await _companyRatingRepository.SaveChangesAsync();
    }

    private CompanyReviewResponseDto MapToDto(CompanyReview review, bool hasUpvoted)
    {
        var authorName = "Anonymous Candidate";
        if (review.User != null && review.User.Candidate != null)
        {
            authorName = review.User.Candidate.FullName;
        }

        return new CompanyReviewResponseDto
        {
            Id = review.Id,
            CompanyId = review.CompanyId,
            UserId = review.UserId,
            AuthorName = authorName,
            Rating = review.Rating,
            Title = review.Title,
            ReviewText = review.ReviewText,
            JobTitle = review.JobTitle,
            InterviewExperience = review.InterviewExperience,
            SalaryOffered = review.SalaryOffered,
            HiringProcessDuration = review.HiringProcessDuration,
            CompanyResponse = review.CompanyResponse,
            HelpfulCount = review.HelpfulCount,
            IsVerified = review.IsVerified,
            HasUpvoted = hasUpvoted,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }

    private async Task<CompanyReviewResponseDto> MapToDtoAsync(CompanyReview review, Guid currentUserId)
    {
        var hasUpvoted = false;
        if (currentUserId != Guid.Empty)
        {
            hasUpvoted = await _companyReviewRepository.HasUserUpvotedAsync(review.Id, currentUserId);
        }
        return MapToDto(review, hasUpvoted);
    }
}
