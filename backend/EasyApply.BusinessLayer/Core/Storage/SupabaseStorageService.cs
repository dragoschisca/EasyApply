using EasyApply.BusinessLayer.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace EasyApply.BusinessLayer.Core.Storage;

public class SupabaseStorageService : ISupabaseStorageService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SupabaseStorageService> _logger;
    private readonly string _supabaseUrl;
    private readonly string _supabaseKey;

    public SupabaseStorageService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<SupabaseStorageService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;

        _supabaseUrl = (configuration["Supabase:Url"]
                        ?? Environment.GetEnvironmentVariable("SUPABASE_URL")
                        ?? string.Empty).TrimEnd('/');

        _supabaseKey = configuration["Supabase:Key"]
                       ?? Environment.GetEnvironmentVariable("SUPABASE_KEY")
                       ?? string.Empty;

        if (string.IsNullOrEmpty(_supabaseUrl) || string.IsNullOrEmpty(_supabaseKey))
        {
            throw new InvalidOperationException("Supabase URL or Key is missing in configuration/environment variables.");
        }

        _logger.LogInformation("SupabaseStorageService initialized with URL: {Url}", _supabaseUrl);
    }

    public async Task<string> UploadFileAsync(string bucket, string fileName, Stream content)
    {
        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_supabaseKey}");
        client.DefaultRequestHeaders.Add("apikey", _supabaseKey);

        // Sanitize the filename (remove spaces and special characters)
        var safeFileName = fileName
            .Replace(" ", "_")
            .Replace("ș", "s")
            .Replace("ț", "t")
            .Replace("ă", "a")
            .Replace("î", "i")
            .Replace("â", "a");

        safeFileName = Regex.Replace(safeFileName, @"[^a-zA-Z0-9_\-\.]", "");

        var encodedFileName = Uri.EscapeDataString(safeFileName);
        var url = $"{_supabaseUrl}/storage/v1/object/{bucket}/{encodedFileName}";

        using var requestContent = new StreamContent(content);
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        var response = await client.PostAsync(url, requestContent);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to upload file to Supabase. Bucket: {Bucket}, File: {FileName}, Status: {Status}, Error: {Error}",
                bucket, safeFileName, response.StatusCode, error);
            throw new Exception($"Failed to upload to Supabase: {response.StatusCode} - {error}");
        }

        _logger.LogDebug("Successfully uploaded file {FileName} to bucket {Bucket}.", safeFileName, bucket);
        return fileName;
    }

    public async Task<Stream> DownloadFileAsync(string bucket, string fileName)
    {
        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_supabaseKey}");
        client.DefaultRequestHeaders.Add("apikey", _supabaseKey);

        var encodedFileName = Uri.EscapeDataString(fileName);
        var url = $"{_supabaseUrl}/storage/v1/object/{bucket}/{encodedFileName}";

        _logger.LogDebug("Downloading file {FileName} from bucket {Bucket}.", fileName, bucket);

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to download file from Supabase. Bucket: {Bucket}, File: {FileName}, Status: {Status}",
                bucket, fileName, response.StatusCode);
            throw new Exception($"Failed to download from Supabase: {response.StatusCode}");
        }

        return await response.Content.ReadAsStreamAsync();
    }

    public async Task DeleteFileAsync(string bucket, string fileName)
    {
        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_supabaseKey}");
        client.DefaultRequestHeaders.Add("apikey", _supabaseKey);

        var url = $"{_supabaseUrl}/storage/v1/object/{bucket}/{fileName}";

        await client.DeleteAsync(url);
        _logger.LogDebug("Deleted file {FileName} from bucket {Bucket}.", fileName, bucket);
    }

    public string GetPublicUrl(string bucket, string fileName)
    {
        return $"{_supabaseUrl}/storage/v1/object/public/{bucket}/{fileName}";
    }
}
