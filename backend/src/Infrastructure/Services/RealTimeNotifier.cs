using Microsoft.AspNetCore.SignalR;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Infrastructure.Hubs;

namespace Rawnex.Infrastructure.Services;

public class RealTimeNotifier : IRealTimeNotifier
{
    private readonly IHubContext<NotificationHub, INotificationHubClient> _notificationHub;
    private readonly IHubContext<TradingHub, ITradingHubClient> _tradingHub;
    private readonly IHubContext<ShipmentTrackingHub, IShipmentTrackingHubClient> _shipmentHub;
    private readonly IHubContext<ChatHub, IChatHubClient> _chatHub;

    public RealTimeNotifier(
        IHubContext<NotificationHub, INotificationHubClient> notificationHub,
        IHubContext<TradingHub, ITradingHubClient> tradingHub,
        IHubContext<ShipmentTrackingHub, IShipmentTrackingHubClient> shipmentHub,
        IHubContext<ChatHub, IChatHubClient> chatHub)
    {
        _notificationHub = notificationHub;
        _tradingHub = tradingHub;
        _shipmentHub = shipmentHub;
        _chatHub = chatHub;
    }

    // ---- Notifications ----
    public async Task SendNotificationAsync(Guid userId, object notification, CancellationToken ct)
    {
        await _notificationHub.Clients.Group($"user-{userId}").ReceiveNotification(notification);
    }

    public async Task UpdateNotificationCountAsync(Guid userId, int unreadCount, CancellationToken ct)
    {
        await _notificationHub.Clients.Group($"user-{userId}").NotificationCountUpdated(unreadCount);
    }

    // ---- Trading ----
    public async Task BroadcastBidAsync(Guid auctionId, object bid, CancellationToken ct)
    {
        await _tradingHub.Clients.Group($"auction-{auctionId}").BidPlaced(bid);
    }

    public async Task BroadcastAuctionStartedAsync(Guid auctionId, CancellationToken ct)
    {
        await _tradingHub.Clients.Group($"auction-{auctionId}").AuctionStarted(auctionId);
    }

    public async Task BroadcastAuctionEndedAsync(Guid auctionId, object? winningBid, CancellationToken ct)
    {
        await _tradingHub.Clients.Group($"auction-{auctionId}").AuctionEnded(auctionId, winningBid);
    }

    public async Task BroadcastListingUpdateAsync(Guid listingId, object listing, CancellationToken ct)
    {
        await _tradingHub.Clients.Group($"listing-{listingId}").ListingUpdated(listingId, listing);
    }

    // ---- Shipment Tracking ----
    public async Task SendShipmentUpdateAsync(Guid shipmentId, Guid buyerUserId, Guid sellerUserId, object tracking, CancellationToken ct)
    {
        await _shipmentHub.Clients.Group($"shipment-{shipmentId}").ShipmentStatusUpdated(shipmentId, tracking);
        // Also notify both parties
        await _notificationHub.Clients.Group($"user-{buyerUserId}").ReceiveNotification(tracking);
        await _notificationHub.Clients.Group($"user-{sellerUserId}").ReceiveNotification(tracking);
    }

    // ---- Chat ----
    public async Task SendChatMessageAsync(Guid conversationId, IEnumerable<Guid> participantUserIds, object message, CancellationToken ct)
    {
        await _chatHub.Clients.Group($"chat-{conversationId}").ReceiveMessage(message);
    }

    public async Task SendTypingIndicatorAsync(Guid conversationId, IEnumerable<Guid> participantUserIds, string userName, bool isTyping, CancellationToken ct)
    {
        if (isTyping)
            await _chatHub.Clients.Group($"chat-{conversationId}").UserTyping(conversationId, userName);
        else
            await _chatHub.Clients.Group($"chat-{conversationId}").UserStoppedTyping(conversationId, userName);
    }
}
