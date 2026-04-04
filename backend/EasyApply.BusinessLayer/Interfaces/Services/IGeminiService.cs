using EasyApply.BusinessLayer.Structure.DTOs.AI;

namespace EasyApply.BusinessLayer.Interfaces.Services;

public interface IGeminiService
{
    Task<CompatibilityResultDto> GetCompatibilityResultAsync(string cvPath, string jobTitle, string jobDescription, string jobSkills);
}
