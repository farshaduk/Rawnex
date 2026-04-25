using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Listings.DTOs;

public record ListingDto(
    Guid Id,
    Guid CompanyId,
    string? CompanyName,
    Guid ProductId,
    string? ProductName,
    ListingType Type,
    ListingStatus Status,
    string Title,
    decimal Quantity,
    string UnitOfMeasure,
    decimal Price,
    Currency PriceCurrency,
    Incoterm Incoterm,
    string? DeliveryLocation,
    DateTime? ExpiresAt,
    DateTime CreatedAt
);

public record ListingDetailDto(
    Guid Id,
    Guid TenantId,
    Guid CompanyId,
    string? CompanyName,
    Guid ProductId,
    string? ProductName,
    ListingType Type,
    ListingStatus Status,
    string Title,
    string? Description,
    decimal Quantity,
    string UnitOfMeasure,
    decimal Price,
    Currency PriceCurrency,
    string? PriceUnit,
    decimal? MinOrderQuantity,
    Incoterm Incoterm,
    string? DeliveryLocation,
    int? LeadTimeDays,
    DateTime? ExpiresAt,
    DateTime? DeliveryStartDate,
    DateTime? DeliveryEndDate,
    DateTime CreatedAt
);
