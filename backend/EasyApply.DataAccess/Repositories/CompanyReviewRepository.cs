using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyApply.DataAccess.Data;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.DataAccess.Repositories;

public class CompanyReviewRepository : ICompanyReviewRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyReview?> GetByIdAsync(Guid id)
    {
        return await _context.CompanyReviews
            .Include(r => r.User).ThenInclude(u => u.Candidate)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<CompanyReview>> GetAllAsync()
    {
        return await _context.CompanyReviews
            .AsNoTracking()
            .Include(r => r.User).ThenInclude(u => u.Candidate)
            .ToListAsync();
    }

    public async Task<(IEnumerable<CompanyReview> Items, int TotalCount)> GetPagedAsync(int skip, int take)
    {
        var query = _context.CompanyReviews.AsNoTracking().Include(r => r.User).ThenInclude(u => u.Candidate);
        var total = await query.CountAsync();
        var items = await query.Skip(skip).Take(take).ToListAsync();
        return (items, total);
    }

    public async Task AddAsync(CompanyReview entity)
    {
        await _context.CompanyReviews.AddAsync(entity);
    }

    public Task UpdateAsync(CompanyReview entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.CompanyReviews.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(CompanyReview entity)
    {
        // Soft delete
        entity.DeletedAt = DateTime.UtcNow;
        _context.CompanyReviews.Update(entity);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<CompanyReview> Items, int TotalCount)> GetCompanyReviewsAsync(
        Guid companyId, int page, int size, string? sortBy, int? ratingFilter)
    {
        var query = _context.CompanyReviews
            .AsNoTracking()
            .Include(r => r.User).ThenInclude(u => u.Candidate)
            .Where(r => r.CompanyId == companyId);

        if (ratingFilter.HasValue)
        {
            query = query.Where(r => r.Rating == ratingFilter.Value);
        }

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "rating_desc" => query.OrderByDescending(r => r.Rating).ThenByDescending(r => r.CreatedAt),
            "rating_asc" => query.OrderBy(r => r.Rating).ThenByDescending(r => r.CreatedAt),
            "helpful" => query.OrderByDescending(r => r.HelpfulCount).ThenByDescending(r => r.CreatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt) // Default to latest reviews
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IEnumerable<CompanyReview>> GetUserReviewsAsync(Guid userId)
    {
        return await _context.CompanyReviews
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<CompanyReview?> GetUserReviewForCompanyAsync(Guid userId, Guid companyId)
    {
        return await _context.CompanyReviews
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.UserId == userId && r.CompanyId == companyId);
    }

    public async Task AddHelpfulAsync(CompanyReviewHelpful helpful)
    {
        await _context.CompanyReviewHelpfuls.AddAsync(helpful);
    }

    public async Task RemoveHelpfulAsync(Guid reviewId, Guid userId)
    {
        var upvote = await _context.CompanyReviewHelpfuls
            .FirstOrDefaultAsync(h => h.ReviewId == reviewId && h.UserId == userId);
        if (upvote != null)
        {
            _context.CompanyReviewHelpfuls.Remove(upvote);
        }
    }

    public async Task<bool> HasUserUpvotedAsync(Guid reviewId, Guid userId)
    {
        return await _context.CompanyReviewHelpfuls
            .AnyAsync(h => h.ReviewId == reviewId && h.UserId == userId);
    }

    public async Task<HashSet<Guid>> GetUpvotedReviewIdsAsync(IEnumerable<Guid> reviewIds, Guid userId)
    {
        var ids = await _context.CompanyReviewHelpfuls
            .Where(h => h.UserId == userId && reviewIds.Contains(h.ReviewId))
            .Select(h => h.ReviewId)
            .ToListAsync();
        return new HashSet<Guid>(ids);
    }
}
