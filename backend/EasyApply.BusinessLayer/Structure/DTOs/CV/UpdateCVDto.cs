namespace EasyApply.BusinessLayer.Structure.DTOs.CV;

public class UpdateCVDto
{
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public int? FileSize { get; set; }
    public string? ParsedContent { get; set; }
    public string? Skills { get; set; }
    public string? Experience { get; set; }
    public string? Education { get; set; }
    public bool? IsPrimary { get; set; }
}
