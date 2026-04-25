using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Disputes.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Disputes.Queries;

public class GetDisputeByIdQueryHandler : IRequestHandler<GetDisputeByIdQuery, Result<DisputeDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetDisputeByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<DisputeDetailDto>> Handle(GetDisputeByIdQuery request, CancellationToken ct)
    {
        var d = await _db.Disputes.AsNoTracking()
            .Include(x => x.FiledByCompany)
            .Include(x => x.AgainstCompany)
            .Include(x => x.Evidence)
            .FirstOrDefaultAsync(x => x.Id == request.DisputeId, ct);

        if (d is null) throw new NotFoundException(nameof(Dispute), request.DisputeId);

        return Result<DisputeDetailDto>.Success(new DisputeDetailDto(
            d.Id, d.TenantId, d.DisputeNumber, d.PurchaseOrderId,
            d.FiledByCompanyId, d.FiledByCompany.LegalName, d.FiledByUserId,
            d.AgainstCompanyId, d.AgainstCompany.LegalName,
            d.Status, d.Reason, d.Description, d.ClaimedAmount, d.ClaimedCurrency,
            d.Resolution, d.ResolutionNotes, d.ResolvedAmount, d.ResolvedAt, d.CreatedAt,
            d.Evidence.Select(e => new DisputeEvidenceDto(
                e.Id, e.UploadedByUserId, e.Title, e.Description,
                e.FileUrl, e.MimeType, e.FileSizeBytes, e.CreatedAt)).ToList()));
    }
}

public class GetOrderDisputesQueryHandler : IRequestHandler<GetOrderDisputesQuery, Result<List<DisputeDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetOrderDisputesQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<DisputeDto>>> Handle(GetOrderDisputesQuery request, CancellationToken ct)
    {
        var list = await _db.Disputes.AsNoTracking()
            .Include(d => d.FiledByCompany)
            .Include(d => d.AgainstCompany)
            .Where(d => d.PurchaseOrderId == request.PurchaseOrderId)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new DisputeDto(
                d.Id, d.DisputeNumber, d.PurchaseOrderId,
                d.FiledByCompanyId, d.FiledByCompany.LegalName,
                d.AgainstCompanyId, d.AgainstCompany.LegalName,
                d.Status, d.Reason, d.ClaimedAmount, d.ClaimedCurrency, d.CreatedAt))
            .ToListAsync(ct);

        return Result<List<DisputeDto>>.Success(list);
    }
}
