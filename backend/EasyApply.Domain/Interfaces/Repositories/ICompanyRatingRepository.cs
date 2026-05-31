using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyApply.Domain.Entities;

namespace EasyApply.Domain.Interfaces.Repositories;

public interface ICompanyRatingRepository
{
    Task<CompanyRating?> GetCompanyRatingAsync(Guid companyId);
    Task UpsertRatingAsync(CompanyRating rating);
    Task<IEnumerable<CompanyRating>> GetTopRatedCompaniesAsync(int limit);
    Task SaveChangesAsync();
}
