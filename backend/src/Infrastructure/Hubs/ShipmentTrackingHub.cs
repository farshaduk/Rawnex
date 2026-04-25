using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Hubs;

/// <summary>
/// Real-time shipment tracking hub. Clients join groups by shipment ID.
/// </summary>
[Authorize]
public class ShipmentTrackingHub : Hub<IShipmentTrackingHubClient>
{
    private readonly ILogger<ShipmentTrackingHub> _logger;

    public ShipmentTrackingHub(ILogger<ShipmentTrackingHub> logger)
    {
        _logger = logger;
    }

    public async Task TrackShipment(Guid shipmentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"shipment-{shipmentId}");
        _logger.LogInformation("User {UserId} started tracking shipment {ShipmentId}", Context.UserIdentifier, shipmentId);
    }

    public async Task StopTrackingShipment(Guid shipmentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"shipment-{shipmentId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Shipment tracking hub disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
