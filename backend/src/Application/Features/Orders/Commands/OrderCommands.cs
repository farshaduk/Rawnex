using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Orders.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Orders.Commands;

public record CreatePurchaseOrderCommand(
    Guid BuyerCompanyId,
    Guid SellerCompanyId,
    Guid? NegotiationId,
    Guid? RfqId,
    Incoterm Incoterm,
    string? DeliveryLocation,
    DateTime? RequestedDeliveryDate,
    string? PaymentTerms,
    string? SpecialInstructions,
    Currency Currency,
    List<CreateOrderItemDto> Items
) : IRequest<Result<PurchaseOrderDto>>;

public record CreateOrderItemDto(
    Guid ProductId,
    Guid? ProductVariantId,
    string ProductName,
    string? Sku,
    decimal Quantity,
    string UnitOfMeasure,
    decimal UnitPrice
);

public record ConfirmOrderCommand(Guid OrderId) : IRequest<Result>;

public record CancelOrderCommand(Guid OrderId, string Reason) : IRequest<Result>;

public record ChangeOrderStatusCommand(Guid OrderId, OrderStatus NewStatus) : IRequest<Result>;

public record ApproveOrderStepCommand(Guid OrderId, Guid ApprovalId, bool Approved, string? Comments) : IRequest<Result>;
