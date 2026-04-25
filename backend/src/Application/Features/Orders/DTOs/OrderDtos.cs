using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Orders.DTOs;

public record PurchaseOrderDto(
    Guid Id,
    string OrderNumber,
    Guid BuyerCompanyId,
    string? BuyerCompanyName,
    Guid SellerCompanyId,
    string? SellerCompanyName,
    OrderStatus Status,
    decimal TotalAmount,
    Currency Currency,
    Incoterm Incoterm,
    DateTime? RequestedDeliveryDate,
    DateTime CreatedAt
);

public record PurchaseOrderDetailDto(
    Guid Id,
    Guid TenantId,
    string OrderNumber,
    Guid BuyerCompanyId,
    string? BuyerCompanyName,
    Guid SellerCompanyId,
    string? SellerCompanyName,
    Guid? NegotiationId,
    Guid? RfqId,
    Guid? ContractId,
    OrderStatus Status,
    Incoterm Incoterm,
    string? DeliveryLocation,
    DateTime? RequestedDeliveryDate,
    string? PaymentTerms,
    string? SpecialInstructions,
    decimal SubTotal,
    decimal? TaxAmount,
    decimal? ShippingCost,
    decimal TotalAmount,
    Currency Currency,
    DateTime? ConfirmedAt,
    DateTime? CompletedAt,
    DateTime? CancelledAt,
    string? CancellationReason,
    DateTime CreatedAt,
    IList<PurchaseOrderItemDto> Items,
    IList<OrderApprovalDto> Approvals
);

public record PurchaseOrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string? Sku,
    decimal Quantity,
    string UnitOfMeasure,
    decimal UnitPrice,
    decimal TotalPrice,
    Currency Currency
);

public record OrderApprovalDto(
    Guid Id,
    int StepOrder,
    string StepName,
    Guid? ApproverUserId,
    ApprovalStatus Status,
    string? Comments,
    DateTime? DecidedAt
);
