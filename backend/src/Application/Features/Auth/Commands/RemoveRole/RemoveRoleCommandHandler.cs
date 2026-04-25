using MediatR;
using Microsoft.AspNetCore.Identity;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Auth.Commands.RemoveRole;

public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public RemoveRoleCommandHandler(
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result> Handle(RemoveRoleCommand request, CancellationToken ct)
    {
        // Permission check is handled by the authorization layer via [HasPermission("roles.manage")]

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null || user.IsDeleted)
            throw new NotFoundException(nameof(ApplicationUser), request.UserId);

        if (!await _userManager.IsInRoleAsync(user, request.RoleName))
            return Result.Failure($"User does not have the '{request.RoleName}' role.");

        var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description).ToList().AsReadOnly());

        await _auditService.LogAsync(AuditAction.RoleRemoved, user.Id, user.Email,
            $"Role '{request.RoleName}' removed by {_currentUser.Email}",
            _currentUser.IpAddress, _currentUser.UserAgent, true, ct);

        return Result.Success();
    }
}
