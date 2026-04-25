using MediatR;
using Rawnex.Application.Common.Models;

namespace Rawnex.Application.Features.Auth.Commands.RevokeAllSessions;

/// <summary>Revoke all sessions for the current user (e.g., "log out everywhere").</summary>
public record RevokeAllSessionsCommand(
    string? IpAddress,
    string? UserAgent
) : IRequest<Result>;
