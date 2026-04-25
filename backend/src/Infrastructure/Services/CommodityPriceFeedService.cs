using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Services;

public class CommodityPriceFeedService : ICommodityPriceFeedService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICacheService _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CommodityPriceFeedService> _logger;

    public CommodityPriceFeedService(
        IHttpClientFactory httpClientFactory,
        ICacheService cache,
        IConfiguration configuration,
        ILogger<CommodityPriceFeedService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<CommodityPrice?> GetLatestPriceAsync(string commoditySymbol, string currency = "USD", CancellationToken ct = default)
    {
        var cacheKey = $"commodity:{commoditySymbol}:{currency}";
        var cached = await _cache.GetAsync<CommodityPrice>(cacheKey, ct);
        if (cached is not null)
            return cached;

        var baseUrl = _configuration["ExternalApis:CommodityPriceFeed:BaseUrl"];
        var apiKey = _configuration["ExternalApis:CommodityPriceFeed:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Commodity price feed not configured. Returning null for {Symbol}", commoditySymbol);
            return null;
        }

        try
        {
            var client = _httpClientFactory.CreateClient("CommodityPriceFeed");
            var url = $"{baseUrl}/latest?access_key={apiKey}&base={currency}&symbols={commoditySymbol}";
            var response = await client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);

            if (json.TryGetProperty("rates", out var rates) && rates.TryGetProperty(commoditySymbol, out var rate))
            {
                var price = new CommodityPrice(
                    commoditySymbol,
                    commoditySymbol,
                    1m / rate.GetDecimal(), // API returns 1/price typically
                    currency,
                    DateTime.UtcNow);

                await _cache.SetAsync(cacheKey, price, TimeSpan.FromMinutes(5), ct);
                return price;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch commodity price for {Symbol}", commoditySymbol);
            return null;
        }
    }

    public async Task<IReadOnlyList<CommodityPrice>> GetHistoricalPricesAsync(
        string commoditySymbol, DateTime from, DateTime to, string currency = "USD", CancellationToken ct = default)
    {
        var baseUrl = _configuration["ExternalApis:CommodityPriceFeed:BaseUrl"];
        var apiKey = _configuration["ExternalApis:CommodityPriceFeed:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
            return Array.Empty<CommodityPrice>();

        try
        {
            var client = _httpClientFactory.CreateClient("CommodityPriceFeed");
            var url = $"{baseUrl}/timeframe?access_key={apiKey}&start_date={from:yyyy-MM-dd}&end_date={to:yyyy-MM-dd}&base={currency}&symbols={commoditySymbol}";
            var response = await client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            var prices = new List<CommodityPrice>();

            if (json.TryGetProperty("rates", out var rates))
            {
                foreach (var dateEntry in rates.EnumerateObject())
                {
                    if (dateEntry.Value.TryGetProperty(commoditySymbol, out var rate))
                    {
                        prices.Add(new CommodityPrice(
                            commoditySymbol,
                            commoditySymbol,
                            1m / rate.GetDecimal(),
                            currency,
                            DateTime.Parse(dateEntry.Name)));
                    }
                }
            }

            return prices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch historical prices for {Symbol}", commoditySymbol);
            return Array.Empty<CommodityPrice>();
        }
    }

    public async Task<IReadOnlyList<CommodityPrice>> GetMultiplePricesAsync(
        IEnumerable<string> symbols, string currency = "USD", CancellationToken ct = default)
    {
        var prices = new List<CommodityPrice>();
        foreach (var symbol in symbols)
        {
            var price = await GetLatestPriceAsync(symbol, currency, ct);
            if (price is not null)
                prices.Add(price);
        }
        return prices;
    }
}
