namespace EasyApply.BusinessLayer.Structure.DTOs.Job;

public class SalaryBenchmarkResponse
{
    public decimal MarketAverage { get; set; }
    public double PercentageDifference { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
}