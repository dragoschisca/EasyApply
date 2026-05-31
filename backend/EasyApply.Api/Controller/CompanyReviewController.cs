using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyApply.BusinessLayer.Interfaces.Services;
using EasyApply.BusinessLayer.Structure.DTOs.CompanyReview;
using Microsoft.AspNetCore.Mvc;

namespace EasyApply.Api.Controller;

[ApiController]
[Route("api/companies")]
public class CompanyReviewController : ControllerBase
{
    private readonly ICompanyReviewService _reviewService;

    public CompanyReviewController(ICompanyReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // POST: api/companies/{companyId}/reviews?userId={userId}
    [HttpPost("{companyId:guid}/reviews")]
    public async Task<IActionResult> CreateOrUpdateReview(
        Guid companyId,
        [FromQuery] Guid userId,
        [FromBody] CreateCompanyReviewDto dto)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("userId query parameter is required.");
        }

        var result = await _reviewService.CreateOrUpdateReviewAsync(userId, companyId, dto);
        return CreatedAtAction(nameof(GetById), new { companyId, reviewId = result.Id }, result);
    }

    // GET: api/companies/{companyId}/reviews?userId={userId}
    [HttpGet("{companyId:guid}/reviews")]
    public async Task<IActionResult> GetCompanyReviews(
        Guid companyId,
        [FromQuery] Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] int? ratingFilter = null)
    {
        var filter = new ReviewFilterDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            RatingFilter = ratingFilter
        };

        var (items, totalCount) = await _reviewService.GetCompanyReviewsAsync(userId, companyId, filter);
        return Ok(new { items, totalCount });
    }

    // GET: api/companies/{companyId}/reviews/{reviewId}
    [HttpGet("{companyId:guid}/reviews/{reviewId:guid}")]
    public async Task<IActionResult> GetById(Guid companyId, Guid reviewId, [FromQuery] Guid userId)
    {
        // Internal lookup to fetch a review by ID can be handled by service
        var filter = new ReviewFilterDto { PageNumber = 1, PageSize = 1 };
        var (items, _) = await _reviewService.GetCompanyReviewsAsync(userId, companyId, filter);
        
        var review = System.Linq.Enumerable.FirstOrDefault(items, r => r.Id == reviewId);
        if (review == null)
        {
            return NotFound($"Review with ID {reviewId} not found under company {companyId}.");
        }

        return Ok(review);
    }

    // PUT: api/companies/{companyId}/reviews/{reviewId}?userId={userId}
    [HttpPut("{companyId:guid}/reviews/{reviewId:guid}")]
    public async Task<IActionResult> Update(
        Guid companyId,
        Guid reviewId,
        [FromQuery] Guid userId,
        [FromBody] UpdateCompanyReviewDto dto)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("userId query parameter is required.");
        }

        var result = await _reviewService.UpdateReviewAsync(userId, reviewId, dto);
        return Ok(result);
    }

    // DELETE: api/companies/{companyId}/reviews/{reviewId}?userId={userId}&isAdmin={isAdmin}
    [HttpDelete("{companyId:guid}/reviews/{reviewId:guid}")]
    public async Task<IActionResult> Delete(
        Guid companyId,
        Guid reviewId,
        [FromQuery] Guid userId,
        [FromQuery] bool isAdmin = false)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("userId query parameter is required.");
        }

        await _reviewService.DeleteReviewAsync(userId, reviewId, isAdmin);
        return NoContent();
    }

    // POST: api/companies/{companyId}/reviews/{reviewId}/helpful?userId={userId}
    [HttpPost("{companyId:guid}/reviews/{reviewId:guid}/helpful")]
    public async Task<IActionResult> ToggleHelpful(
        Guid companyId,
        Guid reviewId,
        [FromQuery] Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("userId query parameter is required.");
        }

        await _reviewService.ToggleHelpfulAsync(userId, reviewId);
        return Ok(new { message = "Helpful status updated successfully." });
    }

    // GET: api/companies/{companyId}/rating
    [HttpGet("{companyId:guid}/rating")]
    public async Task<IActionResult> GetCompanyRating(Guid companyId)
    {
        var result = await _reviewService.GetCompanyRatingAsync(companyId);
        if (result == null)
        {
            return NotFound($"No rating summary found for company ID {companyId}.");
        }
        return Ok(result);
    }

    // GET: api/companies/top-rated
    [HttpGet("top-rated")]
    public async Task<IActionResult> GetTopRated([FromQuery] int limit = 5)
    {
        var result = await _reviewService.GetTopRatedCompaniesAsync(limit);
        return Ok(result);
    }

    // POST: api/companies/{companyId}/reviews/{reviewId}/response?userId={userId}
    [HttpPost("{companyId:guid}/reviews/{reviewId:guid}/response")]
    public async Task<IActionResult> SubmitResponse(
        Guid companyId,
        Guid reviewId,
        [FromQuery] Guid userId,
        [FromBody] string responseText)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("userId query parameter is required.");
        }

        await _reviewService.SubmitCompanyResponseAsync(userId, reviewId, responseText);
        return Ok(new { message = "Company response submitted successfully." });
    }
}
