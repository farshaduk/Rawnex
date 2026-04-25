using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Listings.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Listings.Queries;

public class GetListingByIdQueryHandler : IRequestHandler<GetListingByIdQuery, Result<ListingDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetListingByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ListingDetailDto>> Handle(GetListingByIdQuery request, CancellationToken ct)
    {
        var l = await _db.Listings.AsNoTracking()
            .Include(x => x.Company)
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == request.ListingId && !x.IsDeleted, ct);

        if (l is null) throw new NotFoundException(nameof(Listing), request.ListingId);

        return Result<ListingDetailDto>.Success(new ListingDetailDto(
            l.Id, l.TenantId, l.CompanyId, l.Company.LegalName, l.ProductId, l.Product.Name,
            l.Type, l.Status, l.Title, l.Description, l.Quantity, l.UnitOfMeasure,
            l.Price, l.PriceCurrency, l.PriceUnit, l.MinOrderQuantity, l.Incoterm,
            l.DeliveryLocation, l.LeadTimeDays, l.ExpiresAt, l.DeliveryStartDate, l.DeliveryEndDate,
            l.CreatedAt));
    }
}

public class SearchListingsQueryHandler : IRequestHandler<SearchListingsQuery, Result<PaginatedList<ListingDto>>>
{
    private readonly IApplicationDbContext _db;

    public SearchListingsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<ListingDto>>> Handle(SearchListingsQuery request, CancellationToken ct)
    {
        var query = _db.Listings.AsNoTracking()
            .Include(l => l.Company)
            .Include(l => l.Product)
            .Where(l => !l.IsDeleted);

        if (request.Type.HasValue) query = query.Where(l => l.Type == request.Type.Value);
        if (request.CompanyId.HasValue) query = query.Where(l => l.CompanyId == request.CompanyId.Value);
        if (request.ProductId.HasValue) query = query.Where(l => l.ProductId == request.ProductId.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(l => l.Title.Contains(request.SearchTerm));

        var result = await query
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new ListingDto(
                l.Id, l.CompanyId, l.Company.LegalName, l.ProductId, l.Product.Name,
                l.Type, l.Status, l.Title, l.Quantity, l.UnitOfMeasure,
                l.Price, l.PriceCurrency, l.Incoterm, l.DeliveryLocation, l.ExpiresAt, l.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<ListingDto>>.Success(result);
    }
}
