using MediatR;
using Rawnex.Application.Common.Models;

namespace Rawnex.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(
    string RefreshToken,
    string? IpAddress,
    string? UserAgent
) : IRequest<Result>;
