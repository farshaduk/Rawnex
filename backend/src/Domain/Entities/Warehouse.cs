using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Warehouse : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? ContactPhone { get; set; }
    public decimal? CapacityTons { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Company Company { get; set; } = default!;
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}
