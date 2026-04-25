using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Rfqs.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Rfqs.Commands;

public record CreateRfqCommand(
    Guid BuyerCompanyId,
    string Title,
    string? Description,
    RfqVisibility Visibility,
    Guid? CategoryId,
    string? MaterialName,
    string? RequiredSpecsJson,
    decimal? RequiredQuantity,
    string? UnitOfMeasure,
    decimal? BudgetMin,
    decimal? BudgetMax,
    Currency BudgetCurrency,
    Incoterm? PreferredIncoterm,
    string? DeliveryLocation,
    DateTime? DeliveryDeadline,
    DateTime? ResponseDeadline
) : IRequest<Result<RfqDto>>;

public record SubmitRfqResponseCommand(
    Guid RfqId,
    Guid SellerCompanyId,
    decimal ProposedPrice,
    Currency PriceCurrency,
    string? PriceUnit,
    decimal? ProposedQuantity,
    string? UnitOfMeasure,
    Incoterm? Incoterm,
    int? LeadTimeDays,
    string? PaymentTerms,
    string? TechnicalSpecsJson,
    string? Notes,
    DateTime? ValidUntil
) : IRequest<Result<RfqResponseDto>>;

public record AwardRfqCommand(Guid RfqId, Guid AwardedToCompanyId) : IRequest<Result>;

public record CancelRfqCommand(Guid RfqId) : IRequest<Result>;
