using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Shipments.DTOs;

public record ShipmentDto(
    Guid Id,
    string ShipmentNumber,
    Guid PurchaseOrderId,
    Guid SellerCompanyId,
    string? SellerCompanyName,
    Guid BuyerCompanyId,
    string? BuyerCompanyName,
    ShipmentStatus Status,
    TransportMode TransportMode,
    string? CarrierName,
    string? OriginCity,
    string? DestinationCity,
    DateTime? EstimatedArrivalDate,
    DateTime CreatedAt
);

public record ShipmentDetailDto(
    Guid Id,
    Guid TenantId,
    string ShipmentNumber,
    Guid PurchaseOrderId,
    Guid SellerCompanyId,
    string? SellerCompanyName,
    Guid BuyerCompanyId,
    string? BuyerCompanyName,
    ShipmentStatus Status,
    TransportMode TransportMode,
    Incoterm Incoterm,
    string? CarrierName,
    string? CarrierTrackingNumber,
    string? BillOfLadingNumber,
    string? BillOfLadingUrl,
    string? ContainerNumber,
    string? SealNumber,
    string? OriginAddress,
    string? OriginCity,
    string? OriginCountry,
    string? DestinationAddress,
    string? DestinationCity,
    string? DestinationCountry,
    decimal? GrossWeightKg,
    decimal? NetWeightKg,
    int? NumberOfPackages,
    DateTime? EstimatedDepartureDate,
    DateTime? ActualDepartureDate,
    DateTime? EstimatedArrivalDate,
    DateTime? ActualArrivalDate,
    DateTime? DeliveredAt,
    decimal? ShippingCost,
    decimal? InsuranceCost,
    Currency? CostCurrency,
    decimal? CarbonFootprintKg,
    DateTime CreatedAt,
    IList<ShipmentTrackingDto> TrackingEvents,
    IList<BatchDto> Batches
);

public record ShipmentTrackingDto(
    Guid Id,
    ShipmentStatus Status,
    string? Location,
    string? Description,
    DateTime Timestamp
);

public record BatchDto(
    Guid Id,
    Guid ProductId,
    string BatchNumber,
    string? LotNumber,
    decimal Quantity,
    string UnitOfMeasure,
    DateTime? ManufacturedDate,
    DateTime? ExpiryDate,
    string? Origin,
    string? QualityGrade
);

public record FreightQuoteDto(
    Guid Id,
    string CarrierName,
    TransportMode TransportMode,
    string? OriginCity,
    string? DestinationCity,
    decimal QuotedPrice,
    Currency Currency,
    int? EstimatedTransitDays,
    DateTime? ValidUntil,
    bool IsSelected
);
