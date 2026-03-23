namespace EasyApply.Application.Interfaces.Services;

public interface IGeocodingService
{
    Task<(double? Latitude, double? Longitude)> GeocodeAsync(string address);
}
