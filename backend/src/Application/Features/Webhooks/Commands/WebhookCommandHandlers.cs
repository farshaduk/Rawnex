using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Webhooks.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Webhooks.Commands;

public class CreateWebhookSubscriptionCommandHandler : IRequestHandler<CreateWebhookSubscriptionCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateWebhookSubscriptionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateWebhookSubscriptionCommand request, CancellationToken ct)
    {
        var member = await _context.CompanyMembers
            .FirstOrDefaultAsync(m => m.UserId == _currentUser.UserId!.Value, ct);

        if (member == null)
            return Result<Guid>.Failure("User is not a company member.");

        var subscription = new WebhookSubscription
        {
            TenantId = Guid.Empty, // Set by tenant interceptor
            CompanyId = member.CompanyId,
            EventType = request.EventType,
            Url = request.Url,
            Secret = request.Secret,
            IsActive = true
        };

        _context.WebhookSubscriptions.Add(subscription);
        await _context.SaveChangesAsync(ct);
        return Result<Guid>.Success(subscription.Id);
    }
}

public class UpdateWebhookSubscriptionCommandHandler : IRequestHandler<UpdateWebhookSubscriptionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateWebhookSubscriptionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateWebhookSubscriptionCommand request, CancellationToken ct)
    {
        var subscription = await _context.WebhookSubscriptions.FindAsync(new object[] { request.SubscriptionId }, ct);
        if (subscription == null)
            return Result.Failure("Subscription not found.");

        // Verify ownership
        var member = await _context.CompanyMembers
            .FirstOrDefaultAsync(m => m.UserId == _currentUser.UserId!.Value && m.CompanyId == subscription.CompanyId, ct);

        if (member == null)
            return Result.Failure("Access denied.");

        if (request.EventType != null) subscription.EventType = request.EventType;
        if (request.Url != null) subscription.Url = request.Url;
        if (request.Secret != null) subscription.Secret = request.Secret;
        if (request.IsActive.HasValue) subscription.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class DeleteWebhookSubscriptionCommandHandler : IRequestHandler<DeleteWebhookSubscriptionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteWebhookSubscriptionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteWebhookSubscriptionCommand request, CancellationToken ct)
    {
        var subscription = await _context.WebhookSubscriptions.FindAsync(new object[] { request.SubscriptionId }, ct);
        if (subscription == null)
            return Result.Failure("Subscription not found.");

        var member = await _context.CompanyMembers
            .FirstOrDefaultAsync(m => m.UserId == _currentUser.UserId!.Value && m.CompanyId == subscription.CompanyId, ct);

        if (member == null)
            return Result.Failure("Access denied.");

        _context.WebhookSubscriptions.Remove(subscription);
        await _context.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class TestWebhookCommandHandler : IRequestHandler<TestWebhookCommand, Result<WebhookDeliveryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IWebhookDispatcher _webhookDispatcher;

    public TestWebhookCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IWebhookDispatcher webhookDispatcher)
    {
        _context = context;
        _currentUser = currentUser;
        _webhookDispatcher = webhookDispatcher;
    }

    public async Task<Result<WebhookDeliveryDto>> Handle(TestWebhookCommand request, CancellationToken ct)
    {
        var subscription = await _context.WebhookSubscriptions.FindAsync(new object[] { request.SubscriptionId }, ct);
        if (subscription == null)
            return Result<WebhookDeliveryDto>.Failure("Subscription not found.");

        var member = await _context.CompanyMembers
            .FirstOrDefaultAsync(m => m.UserId == _currentUser.UserId!.Value && m.CompanyId == subscription.CompanyId, ct);

        if (member == null)
            return Result<WebhookDeliveryDto>.Failure("Access denied.");

        var delivery = await _webhookDispatcher.DispatchAsync(subscription, "test", "{\"event\":\"test\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}", ct);

        var dto = new WebhookDeliveryDto(
            delivery.Id, delivery.SubscriptionId, delivery.EventType,
            delivery.HttpStatusCode, delivery.IsSuccess, delivery.AttemptNumber,
            delivery.NextRetryAt, delivery.CreatedAt);

        return Result<WebhookDeliveryDto>.Success(dto);
    }
}
