using System;

namespace EasyApply.BusinessLayer.Structure.DTOs.Application;

public class StatusChangeDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string? Feedback { get; set; }
    public string? ChangedBy { get; set; }
}
