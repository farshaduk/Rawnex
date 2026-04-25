using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Payments.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Payments.Commands;

public record CreateEscrowAccountCommand(
    Guid PurchaseOrderId,
    Guid BuyerCompanyId,
    Guid SellerCompanyId,
    decimal TotalAmount,
    Currency Currency,
    List<CreateMilestoneDto>? Milestones
) : IRequest<Result<EscrowAccountDto>>;

public record CreateMilestoneDto(
    MilestoneType Type,
    string Description,
    decimal ReleasePercentage,
    int SortOrder
);

public record FundEscrowCommand(Guid EscrowAccountId, decimal Amount) : IRequest<Result>;

public record CompleteMilestoneCommand(Guid EscrowAccountId, Guid MilestoneId, string? EvidenceUrl, string? Notes) : IRequest<Result>;

public record RecordPaymentCommand(
    Guid? EscrowAccountId,
    Guid? PurchaseOrderId,
    Guid PayerCompanyId,
    Guid PayeeCompanyId,
    PaymentMethod Method,
    decimal Amount,
    Currency Currency,
    string? TransactionId
) : IRequest<Result<PaymentDto>>;

public record CreateInvoiceCommand(
    Guid PurchaseOrderId,
    Guid IssuerCompanyId,
    Guid RecipientCompanyId,
    InvoiceType Type,
    DateTime DueDate,
    decimal? TaxAmount,
    decimal? DiscountAmount,
    Currency Currency,
    string? Notes,
    List<CreateInvoiceItemDto> Items
) : IRequest<Result<InvoiceDto>>;

public record CreateInvoiceItemDto(
    string Description,
    decimal Quantity,
    string UnitOfMeasure,
    decimal UnitPrice
);

public record MarkInvoicePaidCommand(Guid InvoiceId) : IRequest<Result>;
