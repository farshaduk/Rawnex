using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Negotiations.DTOs;

public record NegotiationDto(
    Guid Id,
    Guid BuyerCompanyId,
    string? BuyerCompanyName,
    Guid SellerCompanyId,
    string? SellerCompanyName,
    NegotiationStatus Status,
    string? Subject,
    decimal? AgreedPrice,
    Currency? AgreedCurrency,
    int MessageCount,
    DateTime CreatedAt
);

public record NegotiationDetailDto(
    Guid Id,
    Guid TenantId,
    Guid BuyerCompanyId,
    string? BuyerCompanyName,
    Guid SellerCompanyId,
    string? SellerCompanyName,
    Guid? RfqId,
    Guid? ListingId,
    NegotiationStatus Status,
    string? Subject,
    decimal? AgreedPrice,
    Currency? AgreedCurrency,
    decimal? AgreedQuantity,
    Incoterm? AgreedIncoterm,
    DateTime? AgreedAt,
    DateTime CreatedAt,
    IList<NegotiationMessageDto> Messages
);

public record NegotiationMessageDto(
    Guid Id,
    Guid SenderUserId,
    string? SenderName,
    Guid SenderCompanyId,
    string Content,
    bool IsCounterOffer,
    string? CounterOfferJson,
    DateTime SentAt,
    DateTime? ReadAt
);
