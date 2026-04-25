namespace Rawnex.Application.Common.Interfaces;

/// <summary>
/// Client methods that the Trading SignalR hub can call on connected clients.
/// </summary>
public interface ITradingHubClient
{
    Task BidPlaced(object bid);
    Task AuctionStarted(Guid auctionId);
    Task AuctionEnded(Guid auctionId, object? winningBid);
    Task ListingUpdated(Guid listingId, object listing);
    Task RfqResponseReceived(Guid rfqId, object response);
    Task NegotiationMessageReceived(Guid negotiationId, object message);
    Task PriceAlertTriggered(object alert);
}

/// <summary>
/// Client methods that the Notification SignalR hub can call on connected clients.
/// </summary>
public interface INotificationHubClient
{
    Task ReceiveNotification(object notification);
    Task NotificationCountUpdated(int unreadCount);
}

/// <summary>
/// Client methods that the Shipment Tracking SignalR hub can call on connected clients.
/// </summary>
public interface IShipmentTrackingHubClient
{
    Task ShipmentStatusUpdated(Guid shipmentId, object tracking);
    Task LocationUpdated(Guid shipmentId, string location, DateTime timestamp);
}

/// <summary>
/// Client methods that the Chat SignalR hub can call on connected clients.
/// </summary>
public interface IChatHubClient
{
    Task ReceiveMessage(object message);
    Task UserTyping(Guid conversationId, string userName);
    Task UserStoppedTyping(Guid conversationId, string userName);
    Task ConversationUpdated(Guid conversationId);
}
