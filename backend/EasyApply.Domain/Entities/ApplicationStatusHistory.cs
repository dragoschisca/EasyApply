using EasyApply.Domain.Enums;

namespace EasyApply.Domain.Entities;

public class ApplicationStatusHistory
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? Feedback { get; set; }
    public string? ChangedBy { get; set; }

    // Navigation
    public Application Application { get; set; } = null!;
}
