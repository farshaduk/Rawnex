using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Payments.DTOs;

public record EscrowAccountDto(
    Guid Id,
    Guid PurchaseOrderId,
    Guid BuyerCompanyId,
    string? BuyerCompanyName,
    Guid SellerCompanyId,
    string? SellerCompanyName,
    EscrowStatus Status,
    decimal TotalAmount,
    decimal FundedAmount,
    decimal ReleasedAmount,
    Currency Currency,
    DateTime CreatedAt
);

public record EscrowMilestoneDto(
    Guid Id,
    MilestoneType Type,
    MilestoneStatus Status,
    string Description,
    decimal ReleasePercentage,
    decimal ReleaseAmount,
    int SortOrder,
    DateTime? CompletedAt,
    string? Notes
);

public record PaymentDto(
    Guid Id,
    Guid PayerCompanyId,
    string? PayerCompanyName,
    Guid PayeeCompanyId,
    string? PayeeCompanyName,
    string PaymentReference,
    PaymentMethod Method,
    PaymentStatus Status,
    decimal Amount,
    Currency Currency,
    DateTime? ProcessedAt,
    DateTime CreatedAt
);

public record InvoiceDto(
    Guid Id,
    string InvoiceNumber,
    Guid IssuerCompanyId,
    string? IssuerCompanyName,
    Guid RecipientCompanyId,
    string? RecipientCompanyName,
    InvoiceType Type,
    InvoiceStatus Status,
    DateTime IssueDate,
    DateTime DueDate,
    decimal TotalAmount,
    decimal PaidAmount,
    Currency Currency,
    DateTime CreatedAt
);

public record InvoiceDetailDto(
    Guid Id,
    Guid TenantId,
    string InvoiceNumber,
    Guid PurchaseOrderId,
    Guid IssuerCompanyId,
    string? IssuerCompanyName,
    Guid RecipientCompanyId,
    string? RecipientCompanyName,
    InvoiceType Type,
    InvoiceStatus Status,
    DateTime IssueDate,
    DateTime DueDate,
    decimal SubTotal,
    decimal? TaxAmount,
    decimal? DiscountAmount,
    decimal TotalAmount,
    decimal PaidAmount,
    Currency Currency,
    string? DocumentUrl,
    string? Notes,
    DateTime? PaidAt,
    DateTime CreatedAt,
    IList<InvoiceItemDto> Items
);

public record InvoiceItemDto(
    Guid Id,
    string Description,
    decimal Quantity,
    string UnitOfMeasure,
    decimal UnitPrice,
    decimal TotalPrice
);
