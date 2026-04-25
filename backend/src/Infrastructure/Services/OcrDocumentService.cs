using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Services;

public class OcrDocumentService : IOcrDocumentService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OcrDocumentService> _logger;

    public OcrDocumentService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<OcrDocumentService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<OcrResult> ExtractTextAsync(Stream documentStream, string fileName, CancellationToken ct = default)
    {
        var baseUrl = _configuration["ExternalApis:Ocr:BaseUrl"];
        var apiKey = _configuration["ExternalApis:Ocr:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("OCR service not configured. Returning empty result for {FileName}", fileName);
            return new OcrResult(false, null, new Dictionary<string, string>(), 0);
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Ocr");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", apiKey);

            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(documentStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Add(streamContent, "file", fileName);

            var response = await client.PostAsync($"{baseUrl}/extract", content, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            var extractedText = json.TryGetProperty("text", out var text) ? text.GetString() : null;
            var fields = new Dictionary<string, string>();

            if (json.TryGetProperty("fields", out var fieldsEl))
            {
                foreach (var field in fieldsEl.EnumerateObject())
                {
                    fields[field.Name] = field.Value.GetString() ?? "";
                }
            }

            var confidence = json.TryGetProperty("confidence", out var c) ? c.GetDecimal() : 0;

            return new OcrResult(true, extractedText, fields, confidence);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR extraction failed for {FileName}", fileName);
            return new OcrResult(false, null, new Dictionary<string, string>(), 0);
        }
    }

    public async Task<DocumentVerificationResult> VerifyDocumentAsync(
        Stream documentStream, string fileName, string expectedDocumentType, CancellationToken ct = default)
    {
        var baseUrl = _configuration["ExternalApis:Ocr:BaseUrl"];
        var apiKey = _configuration["ExternalApis:Ocr:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("OCR service not configured. Returning unverified result for {FileName}", fileName);
            return new DocumentVerificationResult(false, false, expectedDocumentType, 0,
                new Dictionary<string, string>(), ["OCR service not configured"]);
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Ocr");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", apiKey);

            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(documentStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Add(streamContent, "file", fileName);
            content.Add(new StringContent(expectedDocumentType), "documentType");

            var response = await client.PostAsync($"{baseUrl}/verify", content, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            var isAuthentic = json.TryGetProperty("isAuthentic", out var auth) && auth.GetBoolean();
            var docType = json.TryGetProperty("documentType", out var dt) ? dt.GetString() ?? expectedDocumentType : expectedDocumentType;
            var confidence = json.TryGetProperty("confidence", out var c) ? c.GetDecimal() : 0;

            var extractedData = new Dictionary<string, string>();
            if (json.TryGetProperty("data", out var data))
            {
                foreach (var field in data.EnumerateObject())
                    extractedData[field.Name] = field.Value.GetString() ?? "";
            }

            var warnings = new List<string>();
            if (json.TryGetProperty("warnings", out var w))
            {
                foreach (var warning in w.EnumerateArray())
                    warnings.Add(warning.GetString() ?? "");
            }

            return new DocumentVerificationResult(true, isAuthentic, docType, confidence, extractedData, warnings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Document verification failed for {FileName}", fileName);
            return new DocumentVerificationResult(false, false, expectedDocumentType, 0,
                new Dictionary<string, string>(), [ex.Message]);
        }
    }
}
