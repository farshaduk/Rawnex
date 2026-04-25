using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Quality.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Quality.Commands;

public class CreateInspectionCommandHandler : IRequestHandler<CreateInspectionCommand, Result<QualityInspectionDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateInspectionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<QualityInspectionDto>> Handle(CreateInspectionCommand request, CancellationToken ct)
    {
        var order = await _db.PurchaseOrders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == request.PurchaseOrderId, ct);
        if (order is null) throw new NotFoundException(nameof(PurchaseOrder), request.PurchaseOrderId);

        var inspection = new QualityInspection
        {
            TenantId = order.TenantId,
            PurchaseOrderId = request.PurchaseOrderId,
            ShipmentId = request.ShipmentId,
            BatchId = request.BatchId,
            InspectorUserId = _currentUser.UserId!.Value,
            Type = request.Type,
            Status = InspectionStatus.Scheduled,
            InspectionDate = request.InspectionDate,
            Notes = request.Notes,
        };

        _db.QualityInspections.Add(inspection);
        await _db.SaveChangesAsync(ct);

        return Result<QualityInspectionDto>.Success(new QualityInspectionDto(
            inspection.Id, inspection.PurchaseOrderId, inspection.ShipmentId,
            inspection.InspectorUserId, inspection.Type, inspection.Status,
            inspection.InspectionDate, inspection.AiQualityScore, inspection.CreatedAt));
    }
}

public class CompleteInspectionCommandHandler : IRequestHandler<CompleteInspectionCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public CompleteInspectionCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(CompleteInspectionCommand request, CancellationToken ct)
    {
        var inspection = await _db.QualityInspections.FirstOrDefaultAsync(i => i.Id == request.InspectionId, ct);
        if (inspection is null) throw new NotFoundException(nameof(QualityInspection), request.InspectionId);

        inspection.Status = InspectionStatus.Passed;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class AddQualityReportCommandHandler : IRequestHandler<AddQualityReportCommand, Result<QualityReportDto>>
{
    private readonly IApplicationDbContext _db;

    public AddQualityReportCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<QualityReportDto>> Handle(AddQualityReportCommand request, CancellationToken ct)
    {
        var inspection = await _db.QualityInspections.AsNoTracking().FirstOrDefaultAsync(i => i.Id == request.InspectionId, ct);
        if (inspection is null) throw new NotFoundException(nameof(QualityInspection), request.InspectionId);

        var report = new QualityReport
        {
            QualityInspectionId = request.InspectionId,
            Title = request.Title,
            Summary = request.Summary,
            DetailedFindingsJson = request.DetailedFindingsJson,
            FileUrl = request.FileUrl,
            PassedOverallCheck = request.PassedOverallCheck,
        };

        _db.QualityReports.Add(report);

        var labResults = new List<LabTestResultDto>();
        if (request.LabTests is not null)
        {
            foreach (var test in request.LabTests)
            {
                var result = new LabTestResult
                {
                    QualityReportId = report.Id,
                    TestName = test.TestName,
                    TestMethod = test.TestMethod,
                    Parameter = test.Parameter,
                    ExpectedValue = test.ExpectedValue,
                    ActualValue = test.ActualValue,
                    Unit = test.Unit,
                    Passed = test.Passed,
                    LabName = test.LabName,
                    TestDate = test.TestDate,
                };
                _db.LabTestResults.Add(result);
                labResults.Add(new LabTestResultDto(
                    result.Id, result.TestName, result.TestMethod, result.Parameter,
                    result.ExpectedValue, result.ActualValue, result.Unit,
                    result.Passed, result.LabName, result.TestDate));
            }
        }

        await _db.SaveChangesAsync(ct);

        return Result<QualityReportDto>.Success(new QualityReportDto(
            report.Id, report.Title, report.Summary, report.FileUrl,
            report.PassedOverallCheck, report.CreatedAt, labResults));
    }
}
