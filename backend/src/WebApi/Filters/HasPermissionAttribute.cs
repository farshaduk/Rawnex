using Microsoft.AspNetCore.Authorization;

namespace Rawnex.WebApi.Filters;

/// <summary>
/// Authorization attribute that wires into the dynamic permission policy provider.
/// Usage: [HasPermission("products.read")]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base($"Permission:{permission}")
    {
    }
}
