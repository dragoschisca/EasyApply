namespace EasyApply.Application.DTOs.SavedJob;

public class CreateSavedJobDto
{
    public Guid CandidateId { get; set; }
    public Guid JobId { get; set; }
}