using MediatR;
using Rawnex.Application.Common.Models;

namespace Rawnex.Application.Features.Auth.Commands.AssignRole;

public record AssignRoleCommand(
    Guid UserId,
    string RoleName
) : IRequest<Result>;
