using System.Net;
using System.Text.Json;
using Rawnex.Application.Common.Exceptions;

namespace Rawnex.WebApi.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new ErrorResponse("Validation Failed", validationEx.Errors)
            ),
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                new ErrorResponse(notFoundEx.Message)
            ),
            ForbiddenAccessException forbiddenEx => (
                HttpStatusCode.Forbidden,
                new ErrorResponse(forbiddenEx.Message)
            ),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                new ErrorResponse("Unauthorized")
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse("An unexpected error occurred.")
            ),
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Handled exception ({StatusCode}): {Message}", (int)statusCode, exception.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        await context.Response.WriteAsync(json);
    }
}

public record ErrorResponse(string Message, IDictionary<string, string[]>? Errors = null);
