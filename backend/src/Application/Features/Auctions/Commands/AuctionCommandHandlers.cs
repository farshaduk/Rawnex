using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auctions.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.Features.Auctions.Commands;

public class CreateAuctionCommandHandler : IRequestHandler<CreateAuctionCommand, Result<AuctionDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateAuctionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<AuctionDto>> Handle(CreateAuctionCommand request, CancellationToken ct)
    {
        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.CompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        var company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.CompanyId, ct);
        if (company is null) throw new NotFoundException(nameof(Company), request.CompanyId);

        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, ct);
        if (product is null) throw new NotFoundException(nameof(Product), request.ProductId);

        var auction = new Auction
        {
            TenantId = company.TenantId,
            CompanyId = request.CompanyId,
            ProductId = request.ProductId,
            Type = request.Type,
            Status = AuctionStatus.Scheduled,
            Title = request.Title,
            Description = request.Description,
            Quantity = request.Quantity,
            UnitOfMeasure = request.UnitOfMeasure,
            StartingPrice = request.StartingPrice,
            ReservePrice = request.ReservePrice,
            PriceCurrency = request.PriceCurrency,
            MinBidIncrement = request.MinBidIncrement,
            ScheduledStartAt = request.ScheduledStartAt,
            ScheduledEndAt = request.ScheduledEndAt,
        };

        _db.Auctions.Add(auction);
        await _db.SaveChangesAsync(ct);

        return Result<AuctionDto>.Success(new AuctionDto(
            auction.Id, auction.CompanyId, company.LegalName, auction.ProductId, product.Name,
            auction.Type, auction.Status, auction.Title, auction.Quantity, auction.UnitOfMeasure,
            auction.StartingPrice, auction.CurrentHighestBid, auction.PriceCurrency,
            auction.ScheduledStartAt, auction.ScheduledEndAt, 0, auction.CreatedAt));
    }
}

public class PlaceBidCommandHandler : IRequestHandler<PlaceBidCommand, Result<AuctionBidDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public PlaceBidCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<AuctionBidDto>> Handle(PlaceBidCommand request, CancellationToken ct)
    {
        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == request.AuctionId, ct);
        if (auction is null) throw new NotFoundException(nameof(Auction), request.AuctionId);

        if (auction.Status != AuctionStatus.Active)
            return Result<AuctionBidDto>.Failure("Auction is not active.");

        if (auction.CompanyId == request.BidderCompanyId)
            return Result<AuctionBidDto>.Failure("Cannot bid on your own auction.");

        if (auction.CurrentHighestBid.HasValue && request.Amount <= auction.CurrentHighestBid.Value)
            return Result<AuctionBidDto>.Failure("Bid must be higher than the current highest bid.");

        if (auction.MinBidIncrement.HasValue && auction.CurrentHighestBid.HasValue &&
            request.Amount - auction.CurrentHighestBid.Value < auction.MinBidIncrement.Value)
            return Result<AuctionBidDto>.Failure($"Minimum bid increment is {auction.MinBidIncrement.Value}.");

        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.BidderCompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        var company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.BidderCompanyId, ct);

        var bid = new AuctionBid
        {
            AuctionId = request.AuctionId,
            BidderCompanyId = request.BidderCompanyId,
            BidderUserId = _currentUser.UserId!.Value,
            Amount = request.Amount,
            Notes = request.Notes,
        };

        auction.CurrentHighestBid = request.Amount;
        bid.AddDomainEvent(new BidPlacedEvent(auction.Id, request.BidderCompanyId, request.Amount));

        _db.AuctionBids.Add(bid);
        await _db.SaveChangesAsync(ct);

        return Result<AuctionBidDto>.Success(new AuctionBidDto(
            bid.Id, bid.BidderCompanyId, company?.LegalName, bid.Amount, bid.BidAt, false));
    }
}

public class StartAuctionCommandHandler : IRequestHandler<StartAuctionCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public StartAuctionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(StartAuctionCommand request, CancellationToken ct)
    {
        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == request.AuctionId, ct);
        if (auction is null) throw new NotFoundException(nameof(Auction), request.AuctionId);

        var isAdmin = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == auction.CompanyId && m.UserId == _currentUser.UserId && m.IsCompanyAdmin, ct);
        if (!isAdmin) throw new ForbiddenAccessException("Only company admins can start auctions.");

        auction.Status = AuctionStatus.Active;
        auction.ActualStartAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class EndAuctionCommandHandler : IRequestHandler<EndAuctionCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public EndAuctionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(EndAuctionCommand request, CancellationToken ct)
    {
        var auction = await _db.Auctions
            .Include(a => a.Bids)
            .FirstOrDefaultAsync(a => a.Id == request.AuctionId, ct);
        if (auction is null) throw new NotFoundException(nameof(Auction), request.AuctionId);

        var isAdmin = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == auction.CompanyId && m.UserId == _currentUser.UserId && m.IsCompanyAdmin, ct);
        if (!isAdmin) throw new ForbiddenAccessException("Only company admins can end auctions.");

        auction.Status = AuctionStatus.Ended;
        auction.ActualEndAt = DateTime.UtcNow;

        var winningBid = auction.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();
        if (winningBid is not null)
        {
            winningBid.IsWinningBid = true;
            auction.WinnerCompanyId = winningBid.BidderCompanyId;
            auction.WinningBidAmount = winningBid.Amount;
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class CancelAuctionCommandHandler : IRequestHandler<CancelAuctionCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CancelAuctionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(CancelAuctionCommand request, CancellationToken ct)
    {
        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == request.AuctionId, ct);
        if (auction is null) throw new NotFoundException(nameof(Auction), request.AuctionId);

        var isAdmin = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == auction.CompanyId && m.UserId == _currentUser.UserId && m.IsCompanyAdmin, ct);
        if (!isAdmin) throw new ForbiddenAccessException("Only company admins can cancel auctions.");

        auction.Status = AuctionStatus.Cancelled;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
