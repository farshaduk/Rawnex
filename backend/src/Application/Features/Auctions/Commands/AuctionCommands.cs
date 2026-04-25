using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auctions.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Auctions.Commands;

public record CreateAuctionCommand(
    Guid CompanyId,
    Guid ProductId,
    AuctionType Type,
    string Title,
    string? Description,
    decimal Quantity,
    string UnitOfMeasure,
    decimal StartingPrice,
    decimal? ReservePrice,
    Currency PriceCurrency,
    decimal? MinBidIncrement,
    DateTime ScheduledStartAt,
    DateTime ScheduledEndAt
) : IRequest<Result<AuctionDto>>;

public record PlaceBidCommand(
    Guid AuctionId,
    Guid BidderCompanyId,
    decimal Amount,
    string? Notes
) : IRequest<Result<AuctionBidDto>>;

public record StartAuctionCommand(Guid AuctionId) : IRequest<Result>;

public record EndAuctionCommand(Guid AuctionId) : IRequest<Result>;

public record CancelAuctionCommand(Guid AuctionId) : IRequest<Result>;
