using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Negotiations.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Negotiations.Commands;

public class StartNegotiationCommandHandler : IRequestHandler<StartNegotiationCommand, Result<NegotiationDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public StartNegotiationCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<NegotiationDto>> Handle(StartNegotiationCommand request, CancellationToken ct)
    {
        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.BuyerCompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of the buyer company.");

        var buyerCompany = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.BuyerCompanyId, ct);
        if (buyerCompany is null) throw new NotFoundException(nameof(Company), request.BuyerCompanyId);

        var sellerCompany = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.SellerCompanyId, ct);
        if (sellerCompany is null) throw new NotFoundException(nameof(Company), request.SellerCompanyId);

        var negotiation = new Negotiation
        {
            TenantId = buyerCompany.TenantId,
            BuyerCompanyId = request.BuyerCompanyId,
            SellerCompanyId = request.SellerCompanyId,
            RfqId = request.RfqId,
            ListingId = request.ListingId,
            Status = NegotiationStatus.Active,
            Subject = request.Subject,
        };

        _db.Negotiations.Add(negotiation);

        var message = new NegotiationMessage
        {
            NegotiationId = negotiation.Id,
            SenderUserId = _currentUser.UserId!.Value,
            SenderCompanyId = request.BuyerCompanyId,
            Content = request.InitialMessage,
        };

        _db.NegotiationMessages.Add(message);
        await _db.SaveChangesAsync(ct);

        return Result<NegotiationDto>.Success(new NegotiationDto(
            negotiation.Id, negotiation.BuyerCompanyId, buyerCompany.LegalName,
            negotiation.SellerCompanyId, sellerCompany.LegalName,
            negotiation.Status, negotiation.Subject, null, null, 1, negotiation.CreatedAt));
    }
}

public class SendNegotiationMessageCommandHandler : IRequestHandler<SendNegotiationMessageCommand, Result<NegotiationMessageDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SendNegotiationMessageCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<NegotiationMessageDto>> Handle(SendNegotiationMessageCommand request, CancellationToken ct)
    {
        var negotiation = await _db.Negotiations.AsNoTracking().FirstOrDefaultAsync(n => n.Id == request.NegotiationId, ct);
        if (negotiation is null) throw new NotFoundException(nameof(Negotiation), request.NegotiationId);

        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.SenderCompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        if (negotiation.BuyerCompanyId != request.SenderCompanyId && negotiation.SellerCompanyId != request.SenderCompanyId)
            throw new ForbiddenAccessException("Your company is not part of this negotiation.");

        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct);

        var message = new NegotiationMessage
        {
            NegotiationId = request.NegotiationId,
            SenderUserId = _currentUser.UserId!.Value,
            SenderCompanyId = request.SenderCompanyId,
            Content = request.Content,
            IsCounterOffer = request.IsCounterOffer,
            CounterOfferJson = request.CounterOfferJson,
        };

        _db.NegotiationMessages.Add(message);
        await _db.SaveChangesAsync(ct);

        return Result<NegotiationMessageDto>.Success(new NegotiationMessageDto(
            message.Id, message.SenderUserId, user?.FullName, message.SenderCompanyId,
            message.Content, message.IsCounterOffer, message.CounterOfferJson,
            message.SentAt, null));
    }
}

public class AcceptNegotiationCommandHandler : IRequestHandler<AcceptNegotiationCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AcceptNegotiationCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(AcceptNegotiationCommand request, CancellationToken ct)
    {
        var negotiation = await _db.Negotiations.FirstOrDefaultAsync(n => n.Id == request.NegotiationId, ct);
        if (negotiation is null) throw new NotFoundException(nameof(Negotiation), request.NegotiationId);

        negotiation.Status = NegotiationStatus.Agreed;
        negotiation.AgreedPrice = request.AgreedPrice;
        negotiation.AgreedCurrency = request.AgreedCurrency;
        negotiation.AgreedQuantity = request.AgreedQuantity;
        negotiation.AgreedIncoterm = request.AgreedIncoterm;
        negotiation.AgreedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class RejectNegotiationCommandHandler : IRequestHandler<RejectNegotiationCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public RejectNegotiationCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(RejectNegotiationCommand request, CancellationToken ct)
    {
        var negotiation = await _db.Negotiations.FirstOrDefaultAsync(n => n.Id == request.NegotiationId, ct);
        if (negotiation is null) throw new NotFoundException(nameof(Negotiation), request.NegotiationId);

        negotiation.Status = NegotiationStatus.Failed;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
