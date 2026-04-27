using EasyApply.Domain.Enums;

namespace EasyApply.Domain.Models.Company;

public class UpdateCompanyDto
{
    public string? CompanyName { get; set; }
    public string? Industry { get; set; }
    public string? CompanySize { get; set; }
    public string? Website { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? Location { get; set; }
    public string? CompanyCulture { get; set; }
    public SubscriptionTier? SubscriptionTier { get; set; }
}
