using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auctions.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Auctions.Queries;

public class GetAuctionByIdQueryHandler : IRequestHandler<GetAuctionByIdQuery, Result<AuctionDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAuctionByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<AuctionDetailDto>> Handle(GetAuctionByIdQuery request, CancellationToken ct)
    {
        var a = await _db.Auctions.AsNoTracking()
            .Include(x => x.Company)
            .Include(x => x.Product)
            .Include(x => x.Bids.OrderByDescending(b => b.Amount)).ThenInclude(b => b.BidderCompany)
            .FirstOrDefaultAsync(x => x.Id == request.AuctionId, ct);

        if (a is null) throw new NotFoundException(nameof(Auction), request.AuctionId);

        return Result<AuctionDetailDto>.Success(new AuctionDetailDto(
            a.Id, a.TenantId, a.CompanyId, a.Company.LegalName, a.ProductId, a.Product.Name,
            a.Type, a.Status, a.Title, a.Description, a.Quantity, a.UnitOfMeasure,
            a.StartingPrice, a.ReservePrice, a.CurrentHighestBid, a.PriceCurrency,
            a.MinBidIncrement, a.ScheduledStartAt, a.ScheduledEndAt,
            a.ActualStartAt, a.ActualEndAt, a.WinnerCompanyId, a.WinningBidAmount, a.CreatedAt,
            a.Bids.Select(b => new AuctionBidDto(
                b.Id, b.BidderCompanyId, b.BidderCompany.LegalName, b.Amount, b.BidAt, b.IsWinningBid)).ToList()));
    }
}

public class SearchAuctionsQueryHandler : IRequestHandler<SearchAuctionsQuery, Result<PaginatedList<AuctionDto>>>
{
    private readonly IApplicationDbContext _db;

    public SearchAuctionsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<AuctionDto>>> Handle(SearchAuctionsQuery request, CancellationToken ct)
    {
        var query = _db.Auctions.AsNoTracking()
            .Include(a => a.Company).Include(a => a.Product).Include(a => a.Bids)
            .AsQueryable();

        if (request.Status.HasValue) query = query.Where(a => a.Status == request.Status.Value);
        if (request.CompanyId.HasValue) query = query.Where(a => a.CompanyId == request.CompanyId.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(a => a.Title.Contains(request.SearchTerm));

        var result = await query
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AuctionDto(
                a.Id, a.CompanyId, a.Company.LegalName, a.ProductId, a.Product.Name,
                a.Type, a.Status, a.Title, a.Quantity, a.UnitOfMeasure,
                a.StartingPrice, a.CurrentHighestBid, a.PriceCurrency,
                a.ScheduledStartAt, a.ScheduledEndAt, a.Bids.Count, a.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<AuctionDto>>.Success(result);
    }
}
