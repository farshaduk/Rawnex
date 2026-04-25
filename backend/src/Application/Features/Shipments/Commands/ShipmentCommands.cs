using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Shipments.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Shipments.Commands;

public record CreateShipmentCommand(
    Guid PurchaseOrderId,
    Guid SellerCompanyId,
    Guid BuyerCompanyId,
    TransportMode TransportMode,
    Incoterm Incoterm,
    string? CarrierName,
    string? CarrierTrackingNumber,
    string? ContainerNumber,
    string? OriginAddress,
    string? OriginCity,
    string? OriginCountry,
    string? DestinationAddress,
    string? DestinationCity,
    string? DestinationCountry,
    decimal? GrossWeightKg,
    int? NumberOfPackages,
    DateTime? EstimatedDepartureDate,
    DateTime? EstimatedArrivalDate,
    decimal? ShippingCost,
    Currency? CostCurrency
) : IRequest<Result<ShipmentDto>>;

public record UpdateShipmentStatusCommand(
    Guid ShipmentId,
    ShipmentStatus Status,
    string? Location,
    string? Description
) : IRequest<Result>;

public record AddBatchToShipmentCommand(
    Guid ShipmentId,
    Guid ProductId,
    string BatchNumber,
    string? LotNumber,
    decimal Quantity,
    string UnitOfMeasure,
    DateTime? ManufacturedDate,
    DateTime? ExpiryDate,
    string? Origin,
    string? QualityGrade,
    string? CoaFileUrl
) : IRequest<Result<BatchDto>>;

public record RequestFreightQuoteCommand(
    Guid PurchaseOrderId,
    string CarrierName,
    TransportMode TransportMode,
    string? OriginCity,
    string? OriginCountry,
    string? DestinationCity,
    string? DestinationCountry,
    decimal QuotedPrice,
    Currency Currency,
    int? EstimatedTransitDays,
    DateTime? ValidUntil
) : IRequest<Result<FreightQuoteDto>>;

public record SelectFreightQuoteCommand(Guid FreightQuoteId) : IRequest<Result>;
