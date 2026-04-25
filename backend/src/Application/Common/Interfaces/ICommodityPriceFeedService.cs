namespace Rawnex.Application.Common.Interfaces;

public interface ICommodityPriceFeedService
{
    Task<CommodityPrice?> GetLatestPriceAsync(string commoditySymbol, string currency = "USD", CancellationToken ct = default);
    Task<IReadOnlyList<CommodityPrice>> GetHistoricalPricesAsync(string commoditySymbol, DateTime from, DateTime to, string currency = "USD", CancellationToken ct = default);
    Task<IReadOnlyList<CommodityPrice>> GetMultiplePricesAsync(IEnumerable<string> symbols, string currency = "USD", CancellationToken ct = default);
}

public record CommodityPrice(string Symbol, string Name, decimal Price, string Currency, DateTime Timestamp);
