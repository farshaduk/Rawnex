using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Ratings.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Ratings.Queries;

public class GetCompanyRatingsQueryHandler : IRequestHandler<GetCompanyRatingsQuery, Result<PaginatedList<RatingDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetCompanyRatingsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<RatingDto>>> Handle(GetCompanyRatingsQuery request, CancellationToken ct)
    {
        var result = await _db.Ratings.AsNoTracking()
            .Include(r => r.ReviewerCompany)
            .Include(r => r.ReviewedCompany)
            .Where(r => r.ReviewedCompanyId == request.CompanyId && r.IsPublic)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RatingDto(
                r.Id, r.PurchaseOrderId, r.ReviewerCompanyId, r.ReviewerCompany.LegalName,
                r.ReviewedCompanyId, r.ReviewedCompany.LegalName, r.OverallScore,
                r.QualityScore, r.DeliveryScore, r.CommunicationScore,
                r.ValueScore, r.Comment, r.IsPublic, r.ResponseComment, r.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<RatingDto>>.Success(result);
    }
}

public class GetOrderRatingQueryHandler : IRequestHandler<GetOrderRatingQuery, Result<RatingDto>>
{
    private readonly IApplicationDbContext _db;

    public GetOrderRatingQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<RatingDto>> Handle(GetOrderRatingQuery request, CancellationToken ct)
    {
        var r = await _db.Ratings.AsNoTracking()
            .Include(x => x.ReviewerCompany)
            .Include(x => x.ReviewedCompany)
            .FirstOrDefaultAsync(x => x.PurchaseOrderId == request.PurchaseOrderId
                && x.ReviewerCompanyId == request.ReviewerCompanyId, ct);

        if (r is null) throw new NotFoundException(nameof(Rating), request.PurchaseOrderId);

        return Result<RatingDto>.Success(new RatingDto(
            r.Id, r.PurchaseOrderId, r.ReviewerCompanyId, r.ReviewerCompany.LegalName,
            r.ReviewedCompanyId, r.ReviewedCompany.LegalName, r.OverallScore,
            r.QualityScore, r.DeliveryScore, r.CommunicationScore,
            r.ValueScore, r.Comment, r.IsPublic, r.ResponseComment, r.CreatedAt));
    }
}
