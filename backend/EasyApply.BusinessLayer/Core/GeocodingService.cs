using EasyApply.BusinessLayer.Interfaces.Services;
using System.Net.Http.Json;
using System.Text.Json;

namespace EasyApply.BusinessLayer.Core;

/// <summary>
/// Geocodes string addresses to lat/lon using OpenStreetMap Nominatim.
/// Rate limit: 1 request/second — acceptable for single-job create/update flows.
/// </summary>
public class GeocodingService : IGeocodingService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GeocodingService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<(double? Latitude, double? Longitude)> GeocodeAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address)) return (null, null);

        try
        {
            var client = _httpClientFactory.CreateClient("Nominatim");
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";
            var content = await client.GetFromJsonAsync<JsonElement[]>(url);

            if (content == null || content.Length == 0) return (null, null);

            var first = content[0];
            if (first.TryGetProperty("lat", out var latProp) &&
                first.TryGetProperty("lon", out var lonProp))
            {
                if (double.TryParse(latProp.GetString(), System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out var lat) &&
                    double.TryParse(lonProp.GetString(), System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out var lon))
                {
                    return (lat, lon);
                }
            }
        }
        catch
        {
            // Geocoding failure is non-fatal — job is still saved, coordinates stay null.
        }

        return (null, null);
    }
}
