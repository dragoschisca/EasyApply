using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyApply.DataAccess.Data;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.DataAccess.Repositories;

public class CompanyRatingRepository : ICompanyRatingRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRatingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyRating?> GetCompanyRatingAsync(Guid companyId)
    {
        return await _context.CompanyRatings
            .FirstOrDefaultAsync(r => r.CompanyId == companyId);
    }

    public async Task UpsertRatingAsync(CompanyRating rating)
    {
        var existing = await _context.CompanyRatings
            .FirstOrDefaultAsync(r => r.CompanyId == rating.CompanyId);

        if (existing == null)
        {
            await _context.CompanyRatings.AddAsync(rating);
        }
        else
        {
            existing.AverageRating = rating.AverageRating;
            existing.TotalReviews = rating.TotalReviews;
            existing.RatingDistribution = rating.RatingDistribution;
            existing.LastUpdated = DateTime.UtcNow;
            _context.CompanyRatings.Update(existing);
        }
    }

    public async Task<IEnumerable<CompanyRating>> GetTopRatedCompaniesAsync(int limit)
    {
        return await _context.CompanyRatings
            .Include(r => r.Company)
            .OrderByDescending(r => r.AverageRating)
            .ThenByDescending(r => r.TotalReviews)
            .Take(limit)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
