using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auth.DTOs;

namespace Rawnex.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken,
    string? IpAddress,
    string? UserAgent
) : IRequest<Result<AuthTokenResponse>>;
