using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Hubs;

/// <summary>
/// Real-time chat hub for negotiation/deal conversations.
/// Clients join groups by conversation/negotiation ID.
/// </summary>
[Authorize]
public class ChatHub : Hub<IChatHubClient>
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinConversation(Guid conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{conversationId}");
        _logger.LogInformation("User {UserId} joined conversation {ConversationId}", Context.UserIdentifier, conversationId);
    }

    public async Task LeaveConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{conversationId}");
    }

    public async Task SendTyping(Guid conversationId)
    {
        await Clients.OthersInGroup($"chat-{conversationId}")
            .UserTyping(conversationId, Context.UserIdentifier ?? "Unknown");
    }

    public async Task SendStopTyping(Guid conversationId)
    {
        await Clients.OthersInGroup($"chat-{conversationId}")
            .UserStoppedTyping(conversationId, Context.UserIdentifier ?? "Unknown");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Chat hub disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
