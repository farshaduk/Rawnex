using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Webhooks.DTOs;

namespace Rawnex.Application.Features.Webhooks.Queries;

public record GetMyWebhookSubscriptionsQuery(int Page = 1, int PageSize = 20) : IRequest<Result<PaginatedList<WebhookSubscriptionDto>>>;

public record GetWebhookDeliveriesQuery(Guid SubscriptionId, int Page = 1, int PageSize = 20) : IRequest<Result<PaginatedList<WebhookDeliveryDto>>>;
