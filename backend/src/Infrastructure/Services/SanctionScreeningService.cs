using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Entities;

namespace Rawnex.Infrastructure.Services;

public class SanctionScreeningService : ISanctionScreeningService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IApplicationDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SanctionScreeningService> _logger;

    public SanctionScreeningService(
        IHttpClientFactory httpClientFactory,
        IApplicationDbContext db,
        IConfiguration configuration,
        ILogger<SanctionScreeningService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<SanctionScreeningResult> ScreenCompanyAsync(string companyName, string country, CancellationToken ct = default)
    {
        return await ScreenAsync(companyName, "entity", null, country, ct);
    }

    public async Task<SanctionScreeningResult> ScreenIndividualAsync(string fullName, string? dateOfBirth, string? country, CancellationToken ct = default)
    {
        return await ScreenAsync(fullName, "individual", dateOfBirth, country, ct);
    }

    private async Task<SanctionScreeningResult> ScreenAsync(string name, string type, string? dob, string? country, CancellationToken ct)
    {
        var baseUrl = _configuration["ExternalApis:SanctionScreening:BaseUrl"];
        var apiKey = _configuration["ExternalApis:SanctionScreening:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Sanction screening API not configured. Returning clean result for {Name}", name);
            return new SanctionScreeningResult(false, 0, Array.Empty<SanctionHit>(), DateTime.UtcNow);
        }

        try
        {
            var client = _httpClientFactory.CreateClient("SanctionScreening");
            client.DefaultRequestHeaders.TryAddWithoutValidation("apiKey", apiKey);

            var payload = new
            {
                name,
                type,
                dob,
                country,
                minScore = 80
            };

            var response = await client.PostAsJsonAsync($"{baseUrl}/search", payload, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            var hits = new List<SanctionHit>();

            if (json.TryGetProperty("matches", out var matches))
            {
                foreach (var match in matches.EnumerateArray())
                {
                    hits.Add(new SanctionHit(
                        match.TryGetProperty("list", out var list) ? list.GetString() ?? "" : "",
                        match.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "",
                        match.TryGetProperty("score", out var s) ? s.GetDecimal() : 0,
                        match.TryGetProperty("type", out var t) ? t.GetString() : null,
                        match.TryGetProperty("country", out var c) ? c.GetString() : null));
                }
            }

            var isMatch = hits.Count > 0;
            var maxScore = hits.Count > 0 ? hits.Max(h => h.Score) : 0;

            _logger.LogInformation("Sanction screening for {Name}: Match={IsMatch}, Hits={Count}",
                name, isMatch, hits.Count);

            return new SanctionScreeningResult(isMatch, maxScore, hits, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sanction screening failed for {Name}", name);
            return new SanctionScreeningResult(false, 0, Array.Empty<SanctionHit>(), DateTime.UtcNow);
        }
    }
}
