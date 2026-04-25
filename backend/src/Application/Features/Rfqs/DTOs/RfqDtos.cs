using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Rfqs.DTOs;

public record RfqDto(
    Guid Id,
    string RfqNumber,
    Guid BuyerCompanyId,
    string? BuyerCompanyName,
    string Title,
    RfqStatus Status,
    RfqVisibility Visibility,
    string? MaterialName,
    decimal? RequiredQuantity,
    Currency BudgetCurrency,
    DateTime? ResponseDeadline,
    int ResponseCount,
    DateTime CreatedAt
);

public record RfqDetailDto(
    Guid Id,
    Guid TenantId,
    string RfqNumber,
    Guid BuyerCompanyId,
    string? BuyerCompanyName,
    string Title,
    string? Description,
    RfqStatus Status,
    RfqVisibility Visibility,
    Guid? CategoryId,
    string? CategoryName,
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
    DateTime? ResponseDeadline,
    DateTime? AwardedAt,
    Guid? AwardedToCompanyId,
    DateTime CreatedAt,
    IList<RfqResponseDto> Responses
);

public record RfqResponseDto(
    Guid Id,
    Guid SellerCompanyId,
    string? SellerCompanyName,
    BidStatus Status,
    decimal ProposedPrice,
    Currency PriceCurrency,
    decimal? ProposedQuantity,
    Incoterm? Incoterm,
    int? LeadTimeDays,
    string? PaymentTerms,
    string? Notes,
    DateTime? ValidUntil,
    DateTime CreatedAt
);
