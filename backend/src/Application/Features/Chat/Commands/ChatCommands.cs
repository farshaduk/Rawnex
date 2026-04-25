using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Chat.DTOs;

namespace Rawnex.Application.Features.Chat.Commands;

public record CreateChatConversationCommand(
    Guid? NegotiationId,
    Guid? PurchaseOrderId,
    Guid? DisputeId,
    string? Subject,
    List<Guid> ParticipantUserIds) : IRequest<Result<Guid>>;

public record SendChatMessageCommand(
    Guid ConversationId,
    string Content,
    string? AttachmentUrl,
    string? AttachmentType) : IRequest<Result<ChatMessageDto>>;

public record EditChatMessageCommand(
    Guid MessageId,
    string NewContent) : IRequest<Result>;

public record DeleteChatMessageCommand(Guid MessageId) : IRequest<Result>;

public record MarkChatMessagesReadCommand(Guid ConversationId) : IRequest<Result>;
