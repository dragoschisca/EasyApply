namespace EasyApply.BusinessLayer.Structure.DTOs.SavedJob;

public class SavedJobDto
{
    public Guid Id { get; set; }
    public Guid CandidateId { get; set; }
    public Guid JobId { get; set; }
    public DateTime SavedAt { get; set; }
}
