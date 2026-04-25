namespace Rawnex.Application.Features.Webhooks.DTOs;

public record WebhookSubscriptionDto(
    Guid Id,
    Guid CompanyId,
    string EventType,
    string Url,
    bool IsActive,
    int FailureCount,
    DateTime? LastTriggeredAt,
    DateTime? LastSuccessAt,
    string? LastErrorMessage,
    DateTime CreatedAt);

public record WebhookDeliveryDto(
    Guid Id,
    Guid SubscriptionId,
    string EventType,
    int HttpStatusCode,
    bool IsSuccess,
    int AttemptNumber,
    DateTime? NextRetryAt,
    DateTime CreatedAt);
