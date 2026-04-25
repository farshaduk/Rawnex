using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Services;

public class BiometricVerificationService : IBiometricVerificationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BiometricVerificationService> _logger;

    public BiometricVerificationService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<BiometricVerificationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<BiometricResult> CreateVerificationSessionAsync(Guid userId, string redirectUrl, CancellationToken ct = default)
    {
        var baseUrl = _configuration["ExternalApis:Biometric:BaseUrl"];
        var apiKey = _configuration["ExternalApis:Biometric:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Biometric verification not configured. Returning mock session for user {UserId}", userId);
            return new BiometricResult(
                $"bio_mock_{Guid.NewGuid():N}",
                "created",
                false, null, null, null);
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Biometric");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", apiKey);

            var payload = new
            {
                externalUserId = userId.ToString(),
                redirectUrl,
                level = "identity_verification"
            };

            var response = await client.PostAsJsonAsync($"{baseUrl}/sessions", payload, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            return new BiometricResult(
                json.GetProperty("id").GetString()!,
                json.GetProperty("status").GetString()!,
                false,
                null,
                json.TryGetProperty("url", out var url) ? url.GetString() : null,
                null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Biometric session creation failed for user {UserId}", userId);
            return new BiometricResult("", "error", false, null, null, ex.Message);
        }
    }

    public async Task<BiometricResult> GetVerificationResultAsync(string sessionId, CancellationToken ct = default)
    {
        var baseUrl = _configuration["ExternalApis:Biometric:BaseUrl"];
        var apiKey = _configuration["ExternalApis:Biometric:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
        {
            return new BiometricResult(sessionId, "not_configured", false, null, null, "Service not configured");
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Biometric");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", apiKey);

            var response = await client.GetAsync($"{baseUrl}/sessions/{sessionId}", ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            var status = json.GetProperty("status").GetString()!;
            var isVerified = status == "approved" || status == "completed";
            var confidence = json.TryGetProperty("confidence", out var c) ? c.GetDecimal() : (decimal?)null;

            return new BiometricResult(
                sessionId,
                status,
                isVerified,
                confidence,
                null,
                json.TryGetProperty("rejectionReason", out var r) ? r.GetString() : null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get biometric result for session {SessionId}", sessionId);
            return new BiometricResult(sessionId, "error", false, null, null, ex.Message);
        }
    }
}
