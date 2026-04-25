using Microsoft.AspNetCore.Authorization;

namespace Rawnex.Infrastructure.Authorization;

/// <summary>
/// Requirement that specifies a permission key (e.g. "products.read").
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
