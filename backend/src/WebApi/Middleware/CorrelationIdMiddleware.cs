namespace Rawnex.WebApi.Middleware;

public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey(CorrelationIdHeader))
        {
            context.Request.Headers.Append(CorrelationIdHeader, Guid.NewGuid().ToString());
        }

        var correlationId = context.Request.Headers[CorrelationIdHeader].ToString();
        context.Response.Headers.Append(CorrelationIdHeader, correlationId);

        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
