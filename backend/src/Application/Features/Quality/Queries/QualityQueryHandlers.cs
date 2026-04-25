using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Quality.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Quality.Queries;

public class GetInspectionByIdQueryHandler : IRequestHandler<GetInspectionByIdQuery, Result<QualityInspectionDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetInspectionByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<QualityInspectionDetailDto>> Handle(GetInspectionByIdQuery request, CancellationToken ct)
    {
        var i = await _db.QualityInspections.AsNoTracking()
            .Include(x => x.InspectorUser)
            .Include(x => x.Reports).ThenInclude(r => r.LabTestResults)
            .FirstOrDefaultAsync(x => x.Id == request.InspectionId, ct);

        if (i is null) throw new NotFoundException(nameof(QualityInspection), request.InspectionId);

        return Result<QualityInspectionDetailDto>.Success(new QualityInspectionDetailDto(
            i.Id, i.TenantId, i.PurchaseOrderId, i.ShipmentId, i.BatchId,
            i.InspectorUserId, i.InspectorUser.FullName,
            i.Type, i.Status, i.InspectionDate, i.Notes, i.PhotosJson,
            i.AiQualityScore, i.AiAnalysisJson, i.CreatedAt,
            i.Reports.Select(r => new QualityReportDto(
                r.Id, r.Title, r.Summary, r.FileUrl, r.PassedOverallCheck, r.CreatedAt,
                r.LabTestResults.Select(t => new LabTestResultDto(
                    t.Id, t.TestName, t.TestMethod, t.Parameter,
                    t.ExpectedValue, t.ActualValue, t.Unit,
                    t.Passed, t.LabName, t.TestDate)).ToList())).ToList()));
    }
}

public class GetOrderInspectionsQueryHandler : IRequestHandler<GetOrderInspectionsQuery, Result<List<QualityInspectionDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetOrderInspectionsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<QualityInspectionDto>>> Handle(GetOrderInspectionsQuery request, CancellationToken ct)
    {
        var list = await _db.QualityInspections.AsNoTracking()
            .Where(i => i.PurchaseOrderId == request.PurchaseOrderId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new QualityInspectionDto(
                i.Id, i.PurchaseOrderId, i.ShipmentId, i.InspectorUserId,
                i.Type, i.Status, i.InspectionDate, i.AiQualityScore, i.CreatedAt))
            .ToListAsync(ct);

        return Result<List<QualityInspectionDto>>.Success(list);
    }
}
