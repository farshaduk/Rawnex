using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.EventHandlers;

public class BidPlacedEventHandler : INotificationHandler<BidPlacedEvent>
{
    private readonly IApplicationDbContext _db;
    private readonly IRealTimeNotifier _realTime;
    private readonly INotificationService _notification;
    private readonly ILogger<BidPlacedEventHandler> _logger;

    public BidPlacedEventHandler(
        IApplicationDbContext db,
        IRealTimeNotifier realTime,
        INotificationService notification,
        ILogger<BidPlacedEventHandler> logger)
    {
        _db = db;
        _realTime = realTime;
        _notification = notification;
        _logger = logger;
    }

    public async Task Handle(BidPlacedEvent notification, CancellationToken ct)
    {
        var auction = await _db.Auctions.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == notification.AuctionId, ct);
        if (auction is null) return;

        // Broadcast bid to all auction watchers
        await _realTime.BroadcastBidAsync(notification.AuctionId, new
        {
            notification.AuctionId,
            notification.BidderCompanyId,
            notification.Amount,
            Timestamp = DateTime.UtcNow
        }, ct);

        // Notify auction creator
        var creatorMember = await _db.CompanyMembers
            .Where(m => m.CompanyId == auction.CompanyId)
            .Select(m => m.UserId)
            .FirstOrDefaultAsync(ct);

        if (creatorMember != default)
        {
            await _notification.SendAsync(
                creatorMember,
                auction.TenantId,
                "New Bid Received",
                $"A new bid of {notification.Amount:N2} was placed on your auction.",
                NotificationType.BidUpdate,
                NotificationPriority.High,
                $"/auctions/{notification.AuctionId}",
                null,
                ct);
        }

        _logger.LogInformation("Bid event processed: Auction {AuctionId}, Amount {Amount}",
            notification.AuctionId, notification.Amount);
    }
}
