using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Listings.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Listings.Commands;

public record CreateListingCommand(
    Guid CompanyId,
    Guid ProductId,
    ListingType Type,
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
    DateTime? DeliveryEndDate
) : IRequest<Result<ListingDto>>;

public record UpdateListingCommand(
    Guid ListingId,
    string? Title,
    string? Description,
    decimal? Quantity,
    decimal? Price,
    Currency? PriceCurrency,
    decimal? MinOrderQuantity,
    string? DeliveryLocation,
    int? LeadTimeDays,
    DateTime? ExpiresAt
) : IRequest<Result>;

public record ChangeListingStatusCommand(Guid ListingId, ListingStatus Status) : IRequest<Result>;

public record DeleteListingCommand(Guid ListingId) : IRequest<Result>;
