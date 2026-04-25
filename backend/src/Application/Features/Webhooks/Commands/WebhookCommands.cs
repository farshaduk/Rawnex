using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Webhooks.DTOs;

namespace Rawnex.Application.Features.Webhooks.Commands;

public record CreateWebhookSubscriptionCommand(
    string EventType,
    string Url,
    string? Secret) : IRequest<Result<Guid>>;

public record UpdateWebhookSubscriptionCommand(
    Guid SubscriptionId,
    string? EventType,
    string? Url,
    string? Secret,
    bool? IsActive) : IRequest<Result>;

public record DeleteWebhookSubscriptionCommand(Guid SubscriptionId) : IRequest<Result>;

public record TestWebhookCommand(Guid SubscriptionId) : IRequest<Result<WebhookDeliveryDto>>;
