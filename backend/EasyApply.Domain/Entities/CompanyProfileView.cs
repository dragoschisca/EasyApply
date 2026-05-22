using System;

namespace EasyApply.Domain.Entities;

public class CompanyProfileView
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? ViewerId { get; set; }
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Company Company { get; set; } = null!;
    public User? Viewer { get; set; }
}
