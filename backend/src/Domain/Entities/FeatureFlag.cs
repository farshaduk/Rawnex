using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class FeatureFlag : BaseAuditableEntity
{
    public string Key { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public decimal? RolloutPercentage { get; set; }
    public string? TargetTenantsJson { get; set; }
    public string? TargetRolesJson { get; set; }

    public bool IsEnabledForAll => IsEnabled && RolloutPercentage is null or >= 100;
}
