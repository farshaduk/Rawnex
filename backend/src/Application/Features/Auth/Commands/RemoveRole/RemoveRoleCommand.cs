using MediatR;
using Rawnex.Application.Common.Models;

namespace Rawnex.Application.Features.Auth.Commands.RemoveRole;

public record RemoveRoleCommand(
    Guid UserId,
    string RoleName
) : IRequest<Result>;
