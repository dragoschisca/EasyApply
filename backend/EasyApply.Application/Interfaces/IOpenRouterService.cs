using EasyApply.Application.DTOs.AI;

namespace EasyApply.Application.Interfaces.Services;

public interface IOpenRouterService
{
    Task<CompatibilityResultDto> GetCompatibilityResultAsync(string cvPath, string jobTitle, string jobDescription, string jobSkills);
}
