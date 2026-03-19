namespace EasyApply.Application.DTOs.Application;

public class CreateApplicationDto
{
    public Guid JobId { get; set; }
    public Guid CVId { get; set; }
    public string? CoverLetter { get; set; }
}