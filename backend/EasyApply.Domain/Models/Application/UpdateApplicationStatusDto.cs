namespace EasyApply.Domain.Models.Application;

public class UpdateApplicationStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? RecruiterNotes { get; set; }
}