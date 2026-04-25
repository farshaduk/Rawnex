namespace Rawnex.Application.Common.Interfaces;

/// <summary>
/// Abstraction over SignalR hub context for sending real-time messages without depending on SignalR directly.
/// </summary>
public interface IRealTimeNotifier
{
    // Notifications
    Task SendNotificationAsync(Guid userId, object notification, CancellationToken ct = default);
    Task UpdateNotificationCountAsync(Guid userId, int unreadCount, CancellationToken ct = default);

    // Trading
    Task BroadcastBidAsync(Guid auctionId, object bid, CancellationToken ct = default);
    Task BroadcastAuctionStartedAsync(Guid auctionId, CancellationToken ct = default);
    Task BroadcastAuctionEndedAsync(Guid auctionId, object? winningBid, CancellationToken ct = default);
    Task BroadcastListingUpdateAsync(Guid listingId, object listing, CancellationToken ct = default);

    // Shipment tracking
    Task SendShipmentUpdateAsync(Guid shipmentId, Guid buyerUserId, Guid sellerUserId, object tracking, CancellationToken ct = default);

    // Chat
    Task SendChatMessageAsync(Guid conversationId, IEnumerable<Guid> participantUserIds, object message, CancellationToken ct = default);
    Task SendTypingIndicatorAsync(Guid conversationId, IEnumerable<Guid> participantUserIds, string userName, bool isTyping, CancellationToken ct = default);
}
