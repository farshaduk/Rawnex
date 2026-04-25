using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auth.DTOs;

namespace Rawnex.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password,
    string? DeviceInfo,
    string? IpAddress,
    string? UserAgent
) : IRequest<Result<AuthTokenResponse>>;
