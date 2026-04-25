using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class WebhookSubscription : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public string EventType { get; set; } = default!; // e.g., "order.created", "shipment.delivered"
    public string Url { get; set; } = default!;
    public string? Secret { get; set; }
    public bool IsActive { get; set; } = true;
    public int FailureCount { get; set; }
    public DateTime? LastTriggeredAt { get; set; }
    public DateTime? LastSuccessAt { get; set; }
    public string? LastErrorMessage { get; set; }

    // Navigation
    public Company Company { get; set; } = default!;
}

public class WebhookDelivery : BaseAuditableEntity
{
    public Guid SubscriptionId { get; set; }
    public string EventType { get; set; } = default!;
    public string PayloadJson { get; set; } = default!;
    public int HttpStatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public bool IsSuccess { get; set; }
    public int AttemptNumber { get; set; } = 1;
    public DateTime? NextRetryAt { get; set; }

    // Navigation
    public WebhookSubscription Subscription { get; set; } = default!;
}
