namespace EasyApply.BusinessLayer.Interfaces.Services;

using EasyApply.BusinessLayer.Structure.DTOs.Job;

public interface IRecommendationService
{
    Task<IEnumerable<JobDto>> GetRecommendedJobsAsync(Guid candidateId);
}
