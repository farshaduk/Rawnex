using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Services;

public class PushNotificationService : IPushNotificationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PushNotificationService> _logger;

    public PushNotificationService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<PushNotificationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendToUserAsync(Guid userId, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default)
    {
        var serverKey = _configuration["Firebase:ServerKey"];
        if (string.IsNullOrWhiteSpace(serverKey))
        {
            _logger.LogWarning("Firebase not configured. Push to user {UserId} skipped", userId);
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Firebase");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={serverKey}");

            var payload = new
            {
                to = $"/topics/user-{userId}",
                notification = new { title, body },
                data
            };

            var response = await client.PostAsJsonAsync("https://fcm.googleapis.com/fcm/send", payload, ct);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Push sent to user {UserId}: {Title}", userId, title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send push to user {UserId}", userId);
        }
    }

    public async Task SendToTopicAsync(string topic, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default)
    {
        var serverKey = _configuration["Firebase:ServerKey"];
        if (string.IsNullOrWhiteSpace(serverKey))
        {
            _logger.LogWarning("Firebase not configured. Push to topic {Topic} skipped", topic);
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Firebase");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={serverKey}");

            var payload = new
            {
                to = $"/topics/{topic}",
                notification = new { title, body },
                data
            };

            var response = await client.PostAsJsonAsync("https://fcm.googleapis.com/fcm/send", payload, ct);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Push sent to topic {Topic}: {Title}", topic, title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send push to topic {Topic}", topic);
        }
    }
}
