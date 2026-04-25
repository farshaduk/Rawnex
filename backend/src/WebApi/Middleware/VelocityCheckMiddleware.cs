using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Rawnex.WebApi.Middleware;

/// <summary>
/// Middleware that tracks request velocity per user and blocks users making too many
/// state-changing requests in a short time window.
/// </summary>
public class VelocityCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<VelocityCheckMiddleware> _logger;

    // Sliding window counters per user: userId -> list of request timestamps
    private static readonly ConcurrentDictionary<string, List<DateTime>> _requestLog = new();

    private const int MaxRequestsPerWindow = 100;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public VelocityCheckMiddleware(RequestDelegate next, ILogger<VelocityCheckMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only track state-changing requests
        var method = context.Request.Method;
        if (method is not ("POST" or "PUT" or "PATCH" or "DELETE"))
        {
            await _next(context);
            return;
        }

        var userId = context.User.FindFirst("sub")?.Value
                  ?? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                  ?? context.Connection.RemoteIpAddress?.ToString()
                  ?? "anonymous";

        var key = $"velocity:{userId}";
        var now = DateTime.UtcNow;

        var timestamps = _requestLog.GetOrAdd(key, _ => new List<DateTime>());

        lock (timestamps)
        {
            // Prune old entries
            timestamps.RemoveAll(t => now - t > Window);
            timestamps.Add(now);

            if (timestamps.Count > MaxRequestsPerWindow)
            {
                _logger.LogWarning("Velocity check failed for {UserId}: {Count} requests in {Window}s",
                    userId, timestamps.Count, Window.TotalSeconds);

                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers.Append("Retry-After", "60");
                return;
            }
        }

        await _next(context);
    }
}
