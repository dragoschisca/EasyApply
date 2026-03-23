namespace EasyApply.Application.DTOs.AI;

public class CompatibilityResultDto
{
    public decimal Score { get; set; }
    public List<string> Advantages { get; set; } = new();
    public List<string> Disadvantages { get; set; } = new();
    public string Raw { get; set; } = string.Empty;
}
