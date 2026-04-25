using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Notifications.DTOs;

namespace Rawnex.Application.Features.Notifications.Queries;

public record GetMyNotificationsQuery(int PageNumber = 1, int PageSize = 20)
    : IRequest<Result<PaginatedList<NotificationDto>>>;

public record GetUnreadCountQuery : IRequest<Result<int>>;

public record GetMyPreferencesQuery : IRequest<Result<List<NotificationPreferenceDto>>>;
