namespace EasyApply.BusinessLayer.Structure.DTOs.CV;

public class CreateCVDto
{
    public Guid CandidateId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public int FileSize { get; set; }
    public string? Skills { get; set; }
    public string? Experience { get; set; }
    public string? Education { get; set; }
    public bool IsPrimary { get; set; }
}