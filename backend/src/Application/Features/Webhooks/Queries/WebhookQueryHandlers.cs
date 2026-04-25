using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Webhooks.DTOs;

namespace Rawnex.Application.Features.Webhooks.Queries;

public class GetMyWebhookSubscriptionsQueryHandler : IRequestHandler<GetMyWebhookSubscriptionsQuery, Result<PaginatedList<WebhookSubscriptionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyWebhookSubscriptionsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedList<WebhookSubscriptionDto>>> Handle(GetMyWebhookSubscriptionsQuery request, CancellationToken ct)
    {
        var member = await _context.CompanyMembers
            .FirstOrDefaultAsync(m => m.UserId == _currentUser.UserId!.Value, ct);

        if (member == null)
            return Result<PaginatedList<WebhookSubscriptionDto>>.Failure("User is not a company member.");

        var query = _context.WebhookSubscriptions
            .Where(s => s.CompanyId == member.CompanyId)
            .OrderByDescending(s => s.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new WebhookSubscriptionDto(
                s.Id, s.CompanyId, s.EventType, s.Url, s.IsActive,
                s.FailureCount, s.LastTriggeredAt, s.LastSuccessAt,
                s.LastErrorMessage, s.CreatedAt))
            .ToListAsync(ct);

        return Result<PaginatedList<WebhookSubscriptionDto>>.Success(
            new PaginatedList<WebhookSubscriptionDto>(items, totalCount, request.Page, request.PageSize));
    }
}

public class GetWebhookDeliveriesQueryHandler : IRequestHandler<GetWebhookDeliveriesQuery, Result<PaginatedList<WebhookDeliveryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetWebhookDeliveriesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedList<WebhookDeliveryDto>>> Handle(GetWebhookDeliveriesQuery request, CancellationToken ct)
    {
        // Verify user owns the subscription
        var subscription = await _context.WebhookSubscriptions.FindAsync(new object[] { request.SubscriptionId }, ct);
        if (subscription == null)
            return Result<PaginatedList<WebhookDeliveryDto>>.Failure("Subscription not found.");

        var member = await _context.CompanyMembers
            .FirstOrDefaultAsync(m => m.UserId == _currentUser.UserId!.Value && m.CompanyId == subscription.CompanyId, ct);

        if (member == null)
            return Result<PaginatedList<WebhookDeliveryDto>>.Failure("Access denied.");

        var query = _context.WebhookDeliveries
            .Where(d => d.SubscriptionId == request.SubscriptionId)
            .OrderByDescending(d => d.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new WebhookDeliveryDto(
                d.Id, d.SubscriptionId, d.EventType, d.HttpStatusCode,
                d.IsSuccess, d.AttemptNumber, d.NextRetryAt, d.CreatedAt))
            .ToListAsync(ct);

        return Result<PaginatedList<WebhookDeliveryDto>>.Success(
            new PaginatedList<WebhookDeliveryDto>(items, totalCount, request.Page, request.PageSize));
    }
}
