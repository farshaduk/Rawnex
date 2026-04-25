using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Auctions.DTOs;

public record AuctionDto(
    Guid Id,
    Guid CompanyId,
    string? CompanyName,
    Guid ProductId,
    string? ProductName,
    AuctionType Type,
    AuctionStatus Status,
    string Title,
    decimal Quantity,
    string UnitOfMeasure,
    decimal StartingPrice,
    decimal? CurrentHighestBid,
    Currency PriceCurrency,
    DateTime ScheduledStartAt,
    DateTime ScheduledEndAt,
    int BidCount,
    DateTime CreatedAt
);

public record AuctionDetailDto(
    Guid Id,
    Guid TenantId,
    Guid CompanyId,
    string? CompanyName,
    Guid ProductId,
    string? ProductName,
    AuctionType Type,
    AuctionStatus Status,
    string Title,
    string? Description,
    decimal Quantity,
    string UnitOfMeasure,
    decimal StartingPrice,
    decimal? ReservePrice,
    decimal? CurrentHighestBid,
    Currency PriceCurrency,
    decimal? MinBidIncrement,
    DateTime ScheduledStartAt,
    DateTime ScheduledEndAt,
    DateTime? ActualStartAt,
    DateTime? ActualEndAt,
    Guid? WinnerCompanyId,
    decimal? WinningBidAmount,
    DateTime CreatedAt,
    IList<AuctionBidDto> Bids
);

public record AuctionBidDto(
    Guid Id,
    Guid BidderCompanyId,
    string? BidderCompanyName,
    decimal Amount,
    DateTime BidAt,
    bool IsWinningBid
);
