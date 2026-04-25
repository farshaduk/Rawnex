using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Entities;

namespace Rawnex.Infrastructure.Services;

public class WebhookDispatcher : IWebhookDispatcher
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WebhookDispatcher> _logger;

    public WebhookDispatcher(IApplicationDbContext context, IHttpClientFactory httpClientFactory, ILogger<WebhookDispatcher> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<WebhookDelivery> DispatchAsync(WebhookSubscription subscription, string eventType, string payloadJson, CancellationToken ct)
    {
        var delivery = new WebhookDelivery
        {
            SubscriptionId = subscription.Id,
            EventType = eventType,
            PayloadJson = payloadJson,
            AttemptNumber = 1
        };

        try
        {
            var client = _httpClientFactory.CreateClient("Webhook");
            client.Timeout = TimeSpan.FromSeconds(10);

            var request = new HttpRequestMessage(HttpMethod.Post, subscription.Url);
            request.Content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            request.Headers.Add("X-Webhook-Event", eventType);
            request.Headers.Add("X-Webhook-Delivery", delivery.Id.ToString());

            // Sign payload if secret is configured
            if (!string.IsNullOrEmpty(subscription.Secret))
            {
                var signature = ComputeHmacSha256(payloadJson, subscription.Secret);
                request.Headers.Add("X-Webhook-Signature", $"sha256={signature}");
            }

            var response = await client.SendAsync(request, ct);
            delivery.HttpStatusCode = (int)response.StatusCode;
            delivery.ResponseBody = await response.Content.ReadAsStringAsync(ct);
            delivery.IsSuccess = response.IsSuccessStatusCode;

            // Update subscription stats
            subscription.LastTriggeredAt = DateTime.UtcNow;
            if (delivery.IsSuccess)
            {
                subscription.LastSuccessAt = DateTime.UtcNow;
                subscription.FailureCount = 0;
            }
            else
            {
                subscription.FailureCount++;
                subscription.LastErrorMessage = $"HTTP {delivery.HttpStatusCode}";
                delivery.NextRetryAt = DateTime.UtcNow.AddMinutes(Math.Pow(2, Math.Min(subscription.FailureCount, 8)));

                // Deactivate after 10 consecutive failures
                if (subscription.FailureCount >= 10)
                    subscription.IsActive = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook dispatch failed for subscription {SubscriptionId}", subscription.Id);
            delivery.HttpStatusCode = 0;
            delivery.IsSuccess = false;
            delivery.ResponseBody = ex.Message;
            subscription.FailureCount++;
            subscription.LastTriggeredAt = DateTime.UtcNow;
            subscription.LastErrorMessage = ex.Message;
            delivery.NextRetryAt = DateTime.UtcNow.AddMinutes(Math.Pow(2, Math.Min(subscription.FailureCount, 8)));

            if (subscription.FailureCount >= 10)
                subscription.IsActive = false;
        }

        _context.WebhookDeliveries.Add(delivery);
        await _context.SaveChangesAsync(ct);

        return delivery;
    }

    public async Task DispatchToAllSubscribersAsync(Guid tenantId, string eventType, object payload, CancellationToken ct)
    {
        var subscriptions = await _context.WebhookSubscriptions
            .Where(s => s.TenantId == tenantId && s.EventType == eventType && s.IsActive)
            .ToListAsync(ct);

        var payloadJson = JsonSerializer.Serialize(payload);

        foreach (var subscription in subscriptions)
        {
            await DispatchAsync(subscription, eventType, payloadJson, ct);
        }
    }

    private static string ComputeHmacSha256(string data, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexStringLower(hash);
    }
}
