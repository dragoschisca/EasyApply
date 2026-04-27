using EasyApply.Domain.Enums;
namespace EasyApply.Domain.Entities;

public class CV
{
    public Guid Id { get; set; }
    public Guid CandidateId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public int FileSize { get; set; }
    public string? ParsedContent { get; set; } // content from CV
    public string? Skills { get; set; } // JSON string
    public string? Experience { get; set; } // JSON string
    public string? Education { get; set; } // JSON string
    public bool IsPrimary { get; set; } = false;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Candidate Candidate { get; set; } = null!;
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}