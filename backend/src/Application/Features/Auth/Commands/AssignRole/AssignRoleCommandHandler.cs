using MediatR;
using Microsoft.AspNetCore.Identity;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Auth.Commands.AssignRole;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public AssignRoleCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken ct)
    {
        // Permission check is handled by the authorization layer via [HasPermission("roles.manage")]

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null || user.IsDeleted)
            throw new NotFoundException(nameof(ApplicationUser), request.UserId);

        var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
        if (!roleExists)
            return Result.Failure($"Role '{request.RoleName}' does not exist.");

        if (await _userManager.IsInRoleAsync(user, request.RoleName))
            return Result.Failure($"User already has the '{request.RoleName}' role.");

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description).ToList().AsReadOnly());

        await _auditService.LogAsync(AuditAction.RoleAssigned, user.Id, user.Email,
            $"Role '{request.RoleName}' assigned by {_currentUser.Email}",
            _currentUser.IpAddress, _currentUser.UserAgent, true, ct);

        return Result.Success();
    }
}
