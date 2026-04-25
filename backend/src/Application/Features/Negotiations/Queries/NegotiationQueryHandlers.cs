using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Negotiations.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Negotiations.Queries;

public class GetNegotiationByIdQueryHandler : IRequestHandler<GetNegotiationByIdQuery, Result<NegotiationDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetNegotiationByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<NegotiationDetailDto>> Handle(GetNegotiationByIdQuery request, CancellationToken ct)
    {
        var n = await _db.Negotiations.AsNoTracking()
            .Include(x => x.BuyerCompany)
            .Include(x => x.SellerCompany)
            .Include(x => x.Messages.OrderBy(m => m.SentAt)).ThenInclude(m => m.SenderUser)
            .FirstOrDefaultAsync(x => x.Id == request.NegotiationId, ct);

        if (n is null) throw new NotFoundException(nameof(Negotiation), request.NegotiationId);

        return Result<NegotiationDetailDto>.Success(new NegotiationDetailDto(
            n.Id, n.TenantId, n.BuyerCompanyId, n.BuyerCompany.LegalName,
            n.SellerCompanyId, n.SellerCompany.LegalName,
            n.RfqId, n.ListingId, n.Status, n.Subject,
            n.AgreedPrice, n.AgreedCurrency, n.AgreedQuantity, n.AgreedIncoterm, n.AgreedAt,
            n.CreatedAt,
            n.Messages.Select(m => new NegotiationMessageDto(
                m.Id, m.SenderUserId, m.SenderUser.FullName, m.SenderCompanyId,
                m.Content, m.IsCounterOffer, m.CounterOfferJson, m.SentAt, m.ReadAt)).ToList()));
    }
}

public class GetMyNegotiationsQueryHandler : IRequestHandler<GetMyNegotiationsQuery, Result<PaginatedList<NegotiationDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetMyNegotiationsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<NegotiationDto>>> Handle(GetMyNegotiationsQuery request, CancellationToken ct)
    {
        var result = await _db.Negotiations.AsNoTracking()
            .Include(n => n.BuyerCompany)
            .Include(n => n.SellerCompany)
            .Include(n => n.Messages)
            .Where(n => n.BuyerCompanyId == request.CompanyId || n.SellerCompanyId == request.CompanyId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NegotiationDto(
                n.Id, n.BuyerCompanyId, n.BuyerCompany.LegalName,
                n.SellerCompanyId, n.SellerCompany.LegalName,
                n.Status, n.Subject, n.AgreedPrice, n.AgreedCurrency,
                n.Messages.Count, n.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<NegotiationDto>>.Success(result);
    }
}
