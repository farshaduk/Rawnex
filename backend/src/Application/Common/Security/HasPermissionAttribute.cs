namespace Rawnex.Application.Common.Security;

/// <summary>
/// Marks a controller action as requiring a specific permission.
/// Usage: [HasPermission("products.read")] or [HasPermission("orders.manage")]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class HasPermissionAttribute : Attribute
{
    public string Permission { get; }

    public HasPermissionAttribute(string permission)
    {
        Permission = permission;
    }
}
