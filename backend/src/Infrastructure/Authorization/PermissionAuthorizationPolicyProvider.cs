using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Rawnex.Infrastructure.Authorization;

/// <summary>
/// Dynamically creates authorization policies for permission keys.
/// When [HasPermission("products.read")] is used, this provider creates
/// a policy named "Permission:products.read" on the fly.
/// </summary>
public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public const string PolicyPrefix = "Permission:";

    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options) { }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Check if this is a permission-based policy
        if (policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName[PolicyPrefix.Length..];

            return new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();
        }

        // Fall back to default (for built-in policies)
        return await base.GetPolicyAsync(policyName);
    }
}
