using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auth.DTOs;

namespace Rawnex.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? IpAddress,
    string? UserAgent
) : IRequest<Result<UserDto>>;
