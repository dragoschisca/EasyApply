using EasyApply.Domain.Enums;

namespace EasyApply.BusinessLayer.Structure.DTOs.Company;

public class CreateCompanyDto
{
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? CompanySize { get; set; }
    public string? Website { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? Location { get; set; }
}