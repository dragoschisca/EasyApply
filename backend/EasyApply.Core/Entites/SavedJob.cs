namespace EasyApply.Core.Entites;

public class SavedJob
{
    public Guid Id { get; set; }
    public Guid CandidateId { get; set; }
    public Guid JobId { get; set; }
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Candidate Candidate { get; set; } = null!;
    public Job Job { get; set; } = null!;
}