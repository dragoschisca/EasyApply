namespace EasyApply.Domain.Models.SavedJob;

public class CreateSavedJobDto
{
    public Guid CandidateId { get; set; }
    public Guid JobId { get; set; }
}
