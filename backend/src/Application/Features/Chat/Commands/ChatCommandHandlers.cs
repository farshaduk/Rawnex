using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Chat.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Chat.Commands;

public class CreateChatConversationCommandHandler : IRequestHandler<CreateChatConversationCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateChatConversationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateChatConversationCommand request, CancellationToken ct)
    {
        var conversation = new ChatConversation
        {
            TenantId = Guid.Empty, // Set by tenant interceptor
            NegotiationId = request.NegotiationId,
            PurchaseOrderId = request.PurchaseOrderId,
            DisputeId = request.DisputeId,
            Subject = request.Subject,
            IsActive = true
        };

        // Add the current user as participant
        var currentUserId = _currentUser.UserId!.Value;
        var currentMember = await _context.CompanyMembers
            .FirstOrDefaultAsync(m => m.UserId == currentUserId, ct);

        if (currentMember == null)
            return Result<Guid>.Failure("Current user is not a company member.");

        conversation.Participants.Add(new ChatParticipant
        {
            UserId = currentUserId,
            CompanyId = currentMember.CompanyId,
            IsActive = true
        });

        // Add other participants
        foreach (var userId in request.ParticipantUserIds.Where(id => id != currentUserId))
        {
            var member = await _context.CompanyMembers
                .FirstOrDefaultAsync(m => m.UserId == userId, ct);

            if (member != null)
            {
                conversation.Participants.Add(new ChatParticipant
                {
                    UserId = userId,
                    CompanyId = member.CompanyId,
                    IsActive = true
                });
            }
        }

        _context.ChatConversations.Add(conversation);
        await _context.SaveChangesAsync(ct);
        return Result<Guid>.Success(conversation.Id);
    }
}

public class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, Result<ChatMessageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IRealTimeNotifier _realTimeNotifier;

    public SendChatMessageCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IRealTimeNotifier realTimeNotifier)
    {
        _context = context;
        _currentUser = currentUser;
        _realTimeNotifier = realTimeNotifier;
    }

    public async Task<Result<ChatMessageDto>> Handle(SendChatMessageCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;

        var participant = await _context.ChatParticipants
            .Include(p => p.Conversation)
            .FirstOrDefaultAsync(p => p.ConversationId == request.ConversationId && p.UserId == userId && p.IsActive, ct);

        if (participant == null)
            return Result<ChatMessageDto>.Failure("You are not a participant in this conversation.");

        var message = new ChatMessage
        {
            ConversationId = request.ConversationId,
            SenderUserId = userId,
            SenderCompanyId = participant.CompanyId,
            Content = request.Content,
            AttachmentUrl = request.AttachmentUrl,
            AttachmentType = request.AttachmentType
        };

        _context.ChatMessages.Add(message);

        // Update conversation last message time
        participant.Conversation.LastMessageAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        var user = await _context.Users.FindAsync(new object[] { userId }, ct);
        var company = await _context.Companies.FindAsync(new object[] { participant.CompanyId }, ct);

        var dto = new ChatMessageDto(
            message.Id, message.ConversationId, message.SenderUserId,
            user?.FirstName + " " + user?.LastName, message.SenderCompanyId,
            company?.LegalName ?? "", message.Content, message.AttachmentUrl,
            message.AttachmentType, false, null, false, message.CreatedAt);

        // Notify other participants via SignalR
        var participantIds = await _context.ChatParticipants
            .Where(p => p.ConversationId == request.ConversationId && p.IsActive && p.UserId != userId)
            .Select(p => p.UserId)
            .ToListAsync(ct);

        await _realTimeNotifier.SendChatMessageAsync(request.ConversationId, participantIds, dto, ct);

        return Result<ChatMessageDto>.Success(dto);
    }
}

public class EditChatMessageCommandHandler : IRequestHandler<EditChatMessageCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public EditChatMessageCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(EditChatMessageCommand request, CancellationToken ct)
    {
        var message = await _context.ChatMessages.FindAsync(new object[] { request.MessageId }, ct);
        if (message == null)
            return Result.Failure("Message not found.");

        if (message.SenderUserId != _currentUser.UserId!.Value)
            return Result.Failure("You can only edit your own messages.");

        message.Content = request.NewContent;
        message.IsEdited = true;
        message.EditedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class DeleteChatMessageCommandHandler : IRequestHandler<DeleteChatMessageCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteChatMessageCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteChatMessageCommand request, CancellationToken ct)
    {
        var message = await _context.ChatMessages.FindAsync(new object[] { request.MessageId }, ct);
        if (message == null)
            return Result.Failure("Message not found.");

        if (message.SenderUserId != _currentUser.UserId!.Value)
            return Result.Failure("You can only delete your own messages.");

        message.IsDeleted = true;
        message.Content = "[Deleted]";

        await _context.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class MarkChatMessagesReadCommandHandler : IRequestHandler<MarkChatMessagesReadCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public MarkChatMessagesReadCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(MarkChatMessagesReadCommand request, CancellationToken ct)
    {
        var participant = await _context.ChatParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == request.ConversationId && p.UserId == _currentUser.UserId!.Value, ct);

        if (participant == null)
            return Result.Failure("You are not a participant in this conversation.");

        participant.LastReadAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return Result.Success();
    }
}
