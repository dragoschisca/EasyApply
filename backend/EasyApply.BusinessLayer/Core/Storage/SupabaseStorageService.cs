using EasyApply.BusinessLayer.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace EasyApply.BusinessLayer.Core.Storage;

public class SupabaseStorageService : ISupabaseStorageService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _supabaseUrl;
    private readonly string _supabaseKey;

    public SupabaseStorageService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;

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

        Console.WriteLine($"[SupabaseStorage] Initialized with URL: {_supabaseUrl}");
        Console.WriteLine($"[SupabaseStorage] Initialized with Key: {_supabaseKey}");
    }
    public async Task<string> UploadFileAsync(string bucket, string fileName, Stream content)
    {
        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_supabaseKey}");
        client.DefaultRequestHeaders.Add("apikey", _supabaseKey);

        var url = $"{_supabaseUrl}/storage/v1/object/{bucket}/{fileName}";
        
        using var requestContent = new StreamContent(content);
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        var response = await client.PostAsync(url, requestContent);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to upload to Supabase: {response.StatusCode} - {error}");
        }

        return fileName;
    }

    public async Task<Stream> DownloadFileAsync(string bucket, string fileName)
    {
        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_supabaseKey}");
        client.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        
        // Ensure fileName is encoded for the URL
        var encodedFileName = Uri.EscapeDataString(fileName);
        var url = $"{_supabaseUrl}/storage/v1/object/{bucket}/{encodedFileName}";
    
        Console.WriteLine($"[SupabaseStorage] Downloading: {url}");
        
        var response = await client.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
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
    }

    public string GetPublicUrl(string bucket, string fileName)
    {
        return $"{_supabaseUrl}/storage/v1/object/public/{bucket}/{fileName}";
    }
}
