namespace EasyApply.Domain.Models.Application;

public class CreateApplicationDto
{
    public Guid JobId { get; set; }
    public Guid CVId { get; set; }
    public string? CoverLetter { get; set; }
}