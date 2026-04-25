using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auth.DTOs;

namespace Rawnex.Application.Features.Auth.Queries.GetUserSessions;

public record GetUserSessionsQuery : IRequest<Result<List<SessionDto>>>;
