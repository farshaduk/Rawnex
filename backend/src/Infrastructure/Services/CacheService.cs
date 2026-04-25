using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        var data = await _cache.GetStringAsync(key, ct);
        if (data is null) return default;

        try
        {
            return JsonSerializer.Deserialize<T>(data);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize cache entry for key {Key}", key);
            await _cache.RemoveAsync(key, ct);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry, CancellationToken ct)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
        };

        var data = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, data, options, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct)
    {
        await _cache.RemoveAsync(key, ct);
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken ct)
    {
        // IDistributedCache doesn't support key scanning.
        // For Redis, use StackExchange.Redis IDatabase.Execute("KEYS", ...) in a real implementation.
        // For now, this is a no-op; individual keys should be explicitly invalidated.
        _logger.LogDebug("RemoveByPrefixAsync called for prefix {Prefix}. No-op with IDistributedCache.", prefix);
        return Task.CompletedTask;
    }
}
