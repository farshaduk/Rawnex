using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Hubs;

/// <summary>
/// Real-time trading hub for auctions, bids, listings, RFQ responses, negotiations.
/// Clients join groups by auction/listing/rfq/negotiation ID.
/// </summary>
[Authorize]
public class TradingHub : Hub<ITradingHubClient>
{
    private readonly ILogger<TradingHub> _logger;

    public TradingHub(ILogger<TradingHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinAuction(Guid auctionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"auction-{auctionId}");
        _logger.LogInformation("User {UserId} joined auction {AuctionId}", Context.UserIdentifier, auctionId);
    }

    public async Task LeaveAuction(Guid auctionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"auction-{auctionId}");
    }

    public async Task JoinListing(Guid listingId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"listing-{listingId}");
    }

    public async Task LeaveListing(Guid listingId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"listing-{listingId}");
    }

    public async Task JoinRfq(Guid rfqId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"rfq-{rfqId}");
    }

    public async Task LeaveRfq(Guid rfqId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"rfq-{rfqId}");
    }

    public async Task JoinNegotiation(Guid negotiationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"negotiation-{negotiationId}");
    }

    public async Task LeaveNegotiation(Guid negotiationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"negotiation-{negotiationId}");
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Trading hub connected: {ConnectionId}, User: {UserId}", Context.ConnectionId, Context.UserIdentifier);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Trading hub disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
