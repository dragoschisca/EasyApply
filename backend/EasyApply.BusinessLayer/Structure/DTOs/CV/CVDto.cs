using System;

namespace EasyApply.BusinessLayer.Structure.DTOs.CV;

public class CVDto
{
    public Guid Id { get; set; }
    public Guid CandidateId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public int FileSize { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsParsed { get; set; }
    public string? ParsedContent { get; set; }
    public string? Skills { get; set; }
    public string? Experience { get; set; }
    public string? Education { get; set; }
    public DateTime UploadedAt { get; set; }
}