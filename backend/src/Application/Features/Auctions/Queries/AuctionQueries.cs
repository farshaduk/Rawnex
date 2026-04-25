using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auctions.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Auctions.Queries;

public record GetAuctionByIdQuery(Guid AuctionId) : IRequest<Result<AuctionDetailDto>>;

public record SearchAuctionsQuery(
    AuctionStatus? Status,
    Guid? CompanyId,
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedList<AuctionDto>>>;
