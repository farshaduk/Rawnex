using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Chat.DTOs;

namespace Rawnex.Application.Features.Chat.Queries;

public class GetMyConversationsQueryHandler : IRequestHandler<GetMyConversationsQuery, Result<PaginatedList<ChatConversationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyConversationsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedList<ChatConversationDto>>> Handle(GetMyConversationsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;

        var query = _context.ChatConversations
            .Include(c => c.Participants).ThenInclude(p => p.User)
            .Include(c => c.Participants).ThenInclude(p => p.Company)
            .Where(c => c.Participants.Any(p => p.UserId == userId && p.IsActive))
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var myParticipantIds = items
            .SelectMany(c => c.Participants)
            .Where(p => p.UserId == userId)
            .ToDictionary(p => p.ConversationId, p => p.LastReadAt);

        var dtos = new List<ChatConversationDto>();
        foreach (var c in items)
        {
            var lastRead = myParticipantIds.GetValueOrDefault(c.Id);
            var unreadCount = await _context.ChatMessages
                .CountAsync(m => m.ConversationId == c.Id && m.CreatedAt > (lastRead ?? DateTime.MinValue) && m.SenderUserId != userId, ct);

            dtos.Add(new ChatConversationDto(
                c.Id, c.NegotiationId, c.PurchaseOrderId, c.DisputeId,
                c.Subject, c.IsActive, c.LastMessageAt,
                c.Participants.Select(p => new ChatParticipantDto(
                    p.Id, p.UserId,
                    p.User != null ? p.User.FirstName + " " + p.User.LastName : "",
                    p.CompanyId,
                    p.Company?.LegalName ?? "",
                    p.IsActive, p.LastReadAt)).ToList(),
                unreadCount));
        }

        return Result<PaginatedList<ChatConversationDto>>.Success(
            new PaginatedList<ChatConversationDto>(dtos, totalCount, request.Page, request.PageSize));
    }
}

public class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, Result<PaginatedList<ChatMessageDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetConversationMessagesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedList<ChatMessageDto>>> Handle(GetConversationMessagesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;

        // Verify user is a participant
        var isParticipant = await _context.ChatParticipants
            .AnyAsync(p => p.ConversationId == request.ConversationId && p.UserId == userId && p.IsActive, ct);

        if (!isParticipant)
            return Result<PaginatedList<ChatMessageDto>>.Failure("You are not a participant in this conversation.");

        var query = _context.ChatMessages
            .Include(m => m.SenderUser)
            .Include(m => m.SenderCompany)
            .Where(m => m.ConversationId == request.ConversationId)
            .OrderByDescending(m => m.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var messages = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new ChatMessageDto(
                m.Id, m.ConversationId, m.SenderUserId,
                m.SenderUser.FirstName + " " + m.SenderUser.LastName,
                m.SenderCompanyId,
                m.SenderCompany.LegalName,
                m.IsDeleted ? "[Deleted]" : m.Content,
                m.IsDeleted ? null : m.AttachmentUrl,
                m.IsDeleted ? null : m.AttachmentType,
                m.IsEdited, m.EditedAt, m.IsDeleted, m.CreatedAt))
            .ToListAsync(ct);

        return Result<PaginatedList<ChatMessageDto>>.Success(
            new PaginatedList<ChatMessageDto>(messages, totalCount, request.Page, request.PageSize));
    }
}
