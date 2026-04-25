using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Rfqs.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Rfqs.Queries;

public class GetRfqByIdQueryHandler : IRequestHandler<GetRfqByIdQuery, Result<RfqDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetRfqByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<RfqDetailDto>> Handle(GetRfqByIdQuery request, CancellationToken ct)
    {
        var r = await _db.Rfqs.AsNoTracking()
            .Include(x => x.BuyerCompany)
            .Include(x => x.Category)
            .Include(x => x.Responses).ThenInclude(resp => resp.SellerCompany)
            .FirstOrDefaultAsync(x => x.Id == request.RfqId, ct);

        if (r is null) throw new NotFoundException(nameof(Rfq), request.RfqId);

        return Result<RfqDetailDto>.Success(new RfqDetailDto(
            r.Id, r.TenantId, r.RfqNumber, r.BuyerCompanyId, r.BuyerCompany.LegalName,
            r.Title, r.Description, r.Status, r.Visibility, r.CategoryId, r.Category?.Name,
            r.MaterialName, r.RequiredSpecsJson, r.RequiredQuantity, r.UnitOfMeasure,
            r.BudgetMin, r.BudgetMax, r.BudgetCurrency, r.PreferredIncoterm,
            r.DeliveryLocation, r.DeliveryDeadline, r.ResponseDeadline, r.AwardedAt, r.AwardedToCompanyId,
            r.CreatedAt,
            r.Responses.Select(resp => new RfqResponseDto(
                resp.Id, resp.SellerCompanyId, resp.SellerCompany.LegalName,
                resp.Status, resp.ProposedPrice, resp.PriceCurrency,
                resp.ProposedQuantity, resp.Incoterm, resp.LeadTimeDays,
                resp.PaymentTerms, resp.Notes, resp.ValidUntil, resp.CreatedAt)).ToList()));
    }
}

public class SearchRfqsQueryHandler : IRequestHandler<SearchRfqsQuery, Result<PaginatedList<RfqDto>>>
{
    private readonly IApplicationDbContext _db;

    public SearchRfqsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<RfqDto>>> Handle(SearchRfqsQuery request, CancellationToken ct)
    {
        var query = _db.Rfqs.AsNoTracking()
            .Include(r => r.BuyerCompany)
            .Include(r => r.Responses)
            .AsQueryable();

        if (request.Status.HasValue) query = query.Where(r => r.Status == request.Status.Value);
        if (request.BuyerCompanyId.HasValue) query = query.Where(r => r.BuyerCompanyId == request.BuyerCompanyId.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(r => r.Title.Contains(request.SearchTerm) || (r.MaterialName != null && r.MaterialName.Contains(request.SearchTerm)));

        var result = await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RfqDto(
                r.Id, r.RfqNumber, r.BuyerCompanyId, r.BuyerCompany.LegalName,
                r.Title, r.Status, r.Visibility, r.MaterialName,
                r.RequiredQuantity, r.BudgetCurrency, r.ResponseDeadline,
                r.Responses.Count, r.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<RfqDto>>.Success(result);
    }
}

public class GetMyRfqResponsesQueryHandler : IRequestHandler<GetMyRfqResponsesQuery, Result<PaginatedList<RfqResponseDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetMyRfqResponsesQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<RfqResponseDto>>> Handle(GetMyRfqResponsesQuery request, CancellationToken ct)
    {
        var result = await _db.RfqResponses.AsNoTracking()
            .Include(r => r.SellerCompany)
            .Where(r => r.SellerCompanyId == request.SellerCompanyId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RfqResponseDto(
                r.Id, r.SellerCompanyId, r.SellerCompany.LegalName,
                r.Status, r.ProposedPrice, r.PriceCurrency,
                r.ProposedQuantity, r.Incoterm, r.LeadTimeDays,
                r.PaymentTerms, r.Notes, r.ValidUntil, r.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<RfqResponseDto>>.Success(result);
    }
}
