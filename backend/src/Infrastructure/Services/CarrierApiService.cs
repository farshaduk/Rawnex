using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;

namespace Rawnex.Infrastructure.Services;

public class CarrierApiService : ICarrierApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CarrierApiService> _logger;

    public CarrierApiService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<CarrierApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<CarrierTrackingResult> TrackShipmentAsync(string carrierName, string trackingNumber, CancellationToken ct = default)
    {
        var baseUrl = _configuration["ExternalApis:Carrier:BaseUrl"];
        var apiKey = _configuration["ExternalApis:Carrier:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Carrier API not configured. Returning empty tracking for {TrackingNumber}", trackingNumber);
            return new CarrierTrackingResult(trackingNumber, "unknown", null, null, Array.Empty<TrackingEvent>());
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Carrier");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", apiKey);

            var response = await client.GetAsync(
                $"{baseUrl}/tracking?carrier={Uri.EscapeDataString(carrierName)}&tracking_number={Uri.EscapeDataString(trackingNumber)}", ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);

            var events = new List<TrackingEvent>();
            if (json.TryGetProperty("events", out var eventsEl))
            {
                foreach (var ev in eventsEl.EnumerateArray())
                {
                    events.Add(new TrackingEvent(
                        ev.TryGetProperty("timestamp", out var ts) ? ts.GetDateTime() : DateTime.UtcNow,
                        ev.TryGetProperty("location", out var loc) ? loc.GetString() ?? "" : "",
                        ev.TryGetProperty("status", out var st) ? st.GetString() ?? "" : "",
                        ev.TryGetProperty("description", out var desc) ? desc.GetString() : null));
                }
            }

            var status = json.TryGetProperty("status", out var s) ? s.GetString() ?? "unknown" : "unknown";
            var location = json.TryGetProperty("currentLocation", out var cl) ? cl.GetString() : null;
            var eta = json.TryGetProperty("estimatedArrival", out var ea) ? ea.GetDateTime() : (DateTime?)null;

            return new CarrierTrackingResult(trackingNumber, status, location, eta, events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Carrier tracking failed for {TrackingNumber}", trackingNumber);
            return new CarrierTrackingResult(trackingNumber, "error", null, null, Array.Empty<TrackingEvent>());
        }
    }

    public async Task<IReadOnlyList<CarrierQuote>> GetFreightQuotesAsync(FreightQuoteRequest request, CancellationToken ct = default)
    {
        var baseUrl = _configuration["ExternalApis:Carrier:BaseUrl"];
        var apiKey = _configuration["ExternalApis:Carrier:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Carrier API not configured. Returning empty quotes");
            return Array.Empty<CarrierQuote>();
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Carrier");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", apiKey);

            var response = await client.PostAsJsonAsync($"{baseUrl}/quotes", new
            {
                request.OriginCity,
                request.OriginCountry,
                request.DestinationCity,
                request.DestinationCountry,
                TransportMode = request.TransportMode.ToString(),
                request.WeightKg,
                request.Packages
            }, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            var quotes = new List<CarrierQuote>();

            if (json.TryGetProperty("quotes", out var quotesEl))
            {
                foreach (var q in quotesEl.EnumerateArray())
                {
                    quotes.Add(new CarrierQuote(
                        q.GetProperty("carrier").GetString()!,
                        q.GetProperty("price").GetDecimal(),
                        q.TryGetProperty("currency", out var cur) ? cur.GetString()! : "USD",
                        q.TryGetProperty("transitDays", out var td) ? td.GetInt32() : 0,
                        q.TryGetProperty("validUntil", out var vu) ? vu.GetDateTime() : DateTime.UtcNow.AddDays(7)));
                }
            }

            return quotes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get freight quotes");
            return Array.Empty<CarrierQuote>();
        }
    }

    public async Task<CarrierBookingResult> BookShipmentAsync(CarrierBookingRequest request, CancellationToken ct = default)
    {
        var baseUrl = _configuration["ExternalApis:Carrier:BaseUrl"];
        var apiKey = _configuration["ExternalApis:Carrier:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Carrier API not configured. Returning mock booking");
            return new CarrierBookingResult(false, null, null, "Carrier API not configured");
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Carrier");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", apiKey);

            var response = await client.PostAsJsonAsync($"{baseUrl}/bookings", new
            {
                request.CarrierName,
                request.OriginCity,
                request.OriginCountry,
                request.DestinationCity,
                request.DestinationCountry,
                TransportMode = request.TransportMode.ToString(),
                request.WeightKg,
                request.Packages,
                request.PickupDate
            }, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            var bookingRef = json.TryGetProperty("bookingReference", out var br) ? br.GetString() : null;
            var trackingNum = json.TryGetProperty("trackingNumber", out var tn) ? tn.GetString() : null;

            return new CarrierBookingResult(true, bookingRef, trackingNum, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Carrier booking failed");
            return new CarrierBookingResult(false, null, null, ex.Message);
        }
    }
}
