using EasyApply.Domains.Enums;
namespace EasyApply.Domains.Entities;

public class Company
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? CompanySize { get; set; }
    public string? Website { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? Location { get; set; }
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Free;
    public DateTime? SubscriptionExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}