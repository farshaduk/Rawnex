using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Invoice : BaseAuditableEntity, IAggregateRoot, ITenantEntity
{
    public Guid TenantId { get; set; }
    public string InvoiceNumber { get; set; } = default!;
    public Guid PurchaseOrderId { get; set; }
    public Guid IssuerCompanyId { get; set; }
    public Guid RecipientCompanyId { get; set; }
    public InvoiceType Type { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }

    // Amounts
    public decimal SubTotal { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public Currency Currency { get; set; }

    // Document
    public string? DocumentUrl { get; set; }
    public string? Notes { get; set; }

    // Payment
    public DateTime? PaidAt { get; set; }

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public Company IssuerCompany { get; set; } = default!;
    public Company RecipientCompany { get; set; } = default!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}
