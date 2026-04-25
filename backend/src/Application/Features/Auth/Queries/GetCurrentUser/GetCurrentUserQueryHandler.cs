using MediatR;
using Microsoft.AspNetCore.Identity;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auth.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;
    private readonly IPermissionService _permissionService;

    public GetCurrentUserQueryHandler(
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUser,
        IPermissionService permissionService)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _permissionService = permissionService;
    }

    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        if (_currentUser.UserId is null)
            return Result<UserDto>.Failure("Not authenticated.");

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user is null || user.IsDeleted)
            return Result<UserDto>.Failure("User not found.");

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _permissionService.GetUserPermissionsAsync(user.Id, ct);

        return Result<UserDto>.Success(new UserDto(
            user.Id, user.Email!, user.FirstName, user.LastName, user.FullName,
            roles, permissions.ToList(), user.IsActive));
    }
}
