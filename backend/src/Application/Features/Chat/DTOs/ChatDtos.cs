namespace Rawnex.Application.Features.Chat.DTOs;

public record ChatConversationDto(
    Guid Id,
    Guid? NegotiationId,
    Guid? PurchaseOrderId,
    Guid? DisputeId,
    string? Subject,
    bool IsActive,
    DateTime? LastMessageAt,
    List<ChatParticipantDto> Participants,
    int UnreadCount);

public record ChatParticipantDto(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid CompanyId,
    string CompanyName,
    bool IsActive,
    DateTime? LastReadAt);

public record ChatMessageDto(
    Guid Id,
    Guid ConversationId,
    Guid SenderUserId,
    string SenderName,
    Guid SenderCompanyId,
    string SenderCompanyName,
    string Content,
    string? AttachmentUrl,
    string? AttachmentType,
    bool IsEdited,
    DateTime? EditedAt,
    bool IsDeleted,
    DateTime CreatedAt);
