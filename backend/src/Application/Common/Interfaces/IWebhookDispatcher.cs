using Rawnex.Domain.Entities;

namespace Rawnex.Application.Common.Interfaces;

public interface IWebhookDispatcher
{
    Task<WebhookDelivery> DispatchAsync(WebhookSubscription subscription, string eventType, string payloadJson, CancellationToken ct = default);
    Task DispatchToAllSubscribersAsync(Guid tenantId, string eventType, object payload, CancellationToken ct = default);
}
