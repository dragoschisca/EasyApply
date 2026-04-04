using EasyApply.BusinessLayer.Interfaces.Services;
using System.Net.Http.Json;
using System.Text.Json;

namespace EasyApply.DataAccess.Services;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "EasyApply-App");
    }

    public async Task<(double? Latitude, double? Longitude)> GeocodeAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address)) return (null, null);

        try
        {
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode) return (null, null);

            var content = await response.Content.ReadFromJsonAsync<JsonElement[]>();
            if (content == null || content.Length == 0) return (null, null);

            var firstResult = content[0];
            if (firstResult.TryGetProperty("lat", out var latProp) && 
                firstResult.TryGetProperty("lon", out var lonProp))
            {
                if (double.TryParse(latProp.GetString(), out var lat) && 
                    double.TryParse(lonProp.GetString(), out var lon))
                {
                    return (lat, lon);
                }
            }
        }
        catch
        {
            // Log error in real app
        }

        return (null, null);
    }
}
