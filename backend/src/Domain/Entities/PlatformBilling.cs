using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class PlatformBilling : BaseAuditableEntity
{
    public Guid TenantId { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public string BillingReference { get; set; } = default!;
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public CommissionType CommissionType { get; set; }
    public decimal CommissionRate { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? InvoiceUrl { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = default!;
    public PurchaseOrder? PurchaseOrder { get; set; }
}
