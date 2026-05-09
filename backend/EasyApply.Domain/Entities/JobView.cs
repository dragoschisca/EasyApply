using System;

namespace EasyApply.Domain.Entities;

public class JobView
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid? ViewerId { get; set; }
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Job Job { get; set; } = null!;
    public User? Viewer { get; set; }
}
