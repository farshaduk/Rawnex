using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Chat.DTOs;

namespace Rawnex.Application.Features.Chat.Queries;

public record GetMyConversationsQuery(int Page = 1, int PageSize = 20) : IRequest<Result<PaginatedList<ChatConversationDto>>>;

public record GetConversationMessagesQuery(Guid ConversationId, int Page = 1, int PageSize = 50) : IRequest<Result<PaginatedList<ChatMessageDto>>>;
