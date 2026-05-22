using System.Text;
using System.Text.Json;
using EasyApply.BusinessLayer.Interfaces.Services;
using EasyApply.BusinessLayer.Structure.DTOs.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace EasyApply.BusinessLayer.Core.AI;

public class GeminiService : IGeminiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GeminiService> _logger;
    private readonly string _apiKey;
    private const string Model = "google/gemini-2.0-flash-001";

    public GeminiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GeminiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiKey = configuration["OpenRouter:ApiKey"]
                  ?? Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")
                  ?? string.Empty;
    }

    public async Task<CompatibilityResultDto> GetCompatibilityResultAsync(Stream cvStream, string jobTitle, string jobDescription, string jobSkills)
    {
        _logger.LogDebug("Starting CV compatibility analysis for job: {JobTitle}", jobTitle);

        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("OpenRouter API key is missing. Cannot perform CV compatibility analysis.");
            return new CompatibilityResultDto();
        }

        string cvText = ExtractText(cvStream);
        if (string.IsNullOrWhiteSpace(cvText))
        {
            _logger.LogWarning("CV text extraction returned empty content.");
            return new CompatibilityResultDto();
        }

        _logger.LogDebug("Extracted {CharCount} characters from CV.", cvText.Length);

        string prompt = $@"
                You are an experienced HR specialist with 20+ years of recruitment experience across multiple industries.

                Evaluate the candidate's CV against the job below and return a compatibility score.

                ---
                JOB TITLE: {jobTitle}

                JOB DESCRIPTION:
                {jobDescription}

                REQUIRED SKILLS:
                {jobSkills}

                CANDIDATE CV:
                {cvText}
                ---

                SCORING GUIDE (adaptive to role seniority):

                - 90-100: Candidate is an excellent match or exceeds the role requirements
                - 80-89:  Candidate is a strong match with solid relevant experience
                - 70-79:  Candidate meets most requirements, minor gaps
                - 50-69:  Candidate partially matches, notable gaps
                - 1-49:   Candidate is not suitable for this role

                IMPORTANT CALIBRATION RULES:
                - First, infer the seniority level of the role (Junior / Mid / Senior / Lead / Architect)
                - Evaluate the candidate RELATIVE to that seniority level
                - Do NOT artificially limit scores to a specific range
                - High scores (85+) should be common for strong matches, not rare
                - If the candidate clearly meets or exceeds the required level, score MUST be 85+

                - If the candidate is OVERQUALIFIED:
                - This is a STRONG POSITIVE, not a disadvantage
                - Increase the score accordingly

                - If the candidate has real production experience with required technologies:
                - This should significantly increase the score

                - Prefer slightly higher scores when strong evidence exists, rather than conservative scoring
                - Only give low scores (<50) when the candidate is clearly irrelevant

                RULES:
                - Be objective and base evaluation strictly on provided information
                - Do NOT penalize for missing information not explicitly required
                - Do NOT penalize candidates for being more experienced than required
                - Do NOT invent experience the candidate doesn't have
                - Adapt your evaluation criteria to the specific industry and role
                - Responses must be in ROMANIAN

                RULES FOR DEZAVANTAJE (CRITICAL):
                - Only list a disadvantage if it is a GENUINE gap relative to the job requirements
                - Do NOT list things that are irrelevant to the job (e.g. language skills when not required)
                - Do NOT list things that are obvious or expected (e.g. junior has less experience than a senior)
                - Do NOT list technologies the job did NOT explicitly require
                - Do NOT list neutral observations dressed up as weaknesses
                - If there are no real weaknesses, omit the DEZAVANTAJE section entirely
                - A disadvantage must answer: This candidate cannot do X, which the job requires

                OUTPUT FORMAT (STRICT — no extra text, no markdown):

                SCORE: <number between 1 and 100>
                AVANTAJE:
                - <advantage 1>
                - <advantage 2 if applicable>
                - <advantage 3 if applicable>
                DEZAVANTAJE:
                - <weakness 1 if applicable>
                - <weakness 2 if applicable>
                - <weakness 3 if applicable>

                RULES FOR LISTS:
                - AVANTAJE: minimum 1, maximum 3 bullet points
                - DEZAVANTAJE: minimum 0, maximum 3 bullet points (omit section entirely if none)
                ";

        try
        {
            string resultText = await SendToOpenRouter(prompt);
            return ParseResponse(resultText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during CV compatibility analysis for job: {JobTitle}", jobTitle);
            return new CompatibilityResultDto();
        }
    }

    private CompatibilityResultDto ParseResponse(string text)
    {
        var result = new CompatibilityResultDto { Raw = text };
        if (string.IsNullOrWhiteSpace(text) || text == "0") return result;

        var lines = text
            .Replace("\r", "")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .ToList();

        string section = "";

        foreach (var line in lines)
        {
            if (line.StartsWith("SCORE:", StringComparison.OrdinalIgnoreCase))
            {
                var scorePart = line.Split(':').Last().Trim();
                if (decimal.TryParse(scorePart, out var score))
                    result.Score = Math.Min(100, Math.Max(0, score));
            }
            else if (line.StartsWith("AVANTAJE", StringComparison.OrdinalIgnoreCase))
            {
                section = "AV";
            }
            else if (line.StartsWith("DEZAVANTAJE", StringComparison.OrdinalIgnoreCase))
            {
                section = "DZ";
            }
            else if (line.StartsWith("-"))
            {
                var item = line.TrimStart('-', ' ').Trim();
                if (section == "AV") result.Advantages.Add(item);
                if (section == "DZ") result.Disadvantages.Add(item);
            }
        }

        return result;
    }

    private string ExtractText(Stream pdfStream)
    {
        try
        {
            pdfStream.Position = 0;

            var text = new StringBuilder();
            using (var reader = new PdfReader(pdfStream))
            using (var pdfDoc = new PdfDocument(reader))
            {
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var page = pdfDoc.GetPage(i);
                    var strategy = new SimpleTextExtractionStrategy();
                    var pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
                    text.AppendLine(pageText);
                }
            }

            var result = text.ToString().Trim();
            _logger.LogDebug("Extracted {CharCount} characters from PDF.", result.Length);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract text from PDF stream.");
            return string.Empty;
        }
    }

    private async Task<string> SendToOpenRouter(string prompt)
    {
        using var client = _httpClientFactory.CreateClient();

        var requestBody = new
        {
            model = Model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        request.Headers.Add("HTTP-Referer", "https://localhost");
        request.Headers.Add("X-Title", "EasyApply");

        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("OpenRouter API returned {StatusCode}. Response: {Response}", response.StatusCode, responseJson);
            return "0";
        }

        try
        {
            using var doc = JsonDocument.Parse(responseJson);
            var text = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return text?.Trim() ?? "0";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse OpenRouter API response.");
            return "0";
        }
    }
}
