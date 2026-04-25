using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Payment : BaseAuditableEntity, IAggregateRoot, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid? EscrowAccountId { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public Guid PayerCompanyId { get; set; }
    public Guid PayeeCompanyId { get; set; }
    public string PaymentReference { get; set; } = default!;
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string? TransactionId { get; set; }
    public string? GatewayResponse { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? FailureReason { get; set; }

    // Navigation
    public EscrowAccount? EscrowAccount { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
    public Company PayerCompany { get; set; } = default!;
    public Company PayeeCompany { get; set; } = default!;
}
