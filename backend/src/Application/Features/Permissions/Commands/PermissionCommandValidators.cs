using FluentValidation;

namespace Rawnex.Application.Features.Permissions.Commands;

public class AssignPermissionsToRoleCommandValidator : AbstractValidator<AssignPermissionsToRoleCommand>
{
    public AssignPermissionsToRoleCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.PermissionIds).NotEmpty().WithMessage("At least one permission must be specified.");
    }
}

public class RevokePermissionsFromRoleCommandValidator : AbstractValidator<RevokePermissionsFromRoleCommand>
{
    public RevokePermissionsFromRoleCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.PermissionIds).NotEmpty().WithMessage("At least one permission must be specified.");
    }
}

public class GrantPermissionToUserCommandValidator : AbstractValidator<GrantPermissionToUserCommand>
{
    public GrantPermissionToUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PermissionId).NotEmpty();
    }
}

public class RevokeUserPermissionCommandValidator : AbstractValidator<RevokeUserPermissionCommand>
{
    public RevokeUserPermissionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PermissionId).NotEmpty();
    }
}
