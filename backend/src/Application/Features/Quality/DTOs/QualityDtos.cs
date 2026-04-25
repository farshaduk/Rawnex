using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Quality.DTOs;

public record QualityInspectionDto(
    Guid Id,
    Guid PurchaseOrderId,
    Guid? ShipmentId,
    Guid InspectorUserId,
    InspectionType Type,
    InspectionStatus Status,
    DateTime? InspectionDate,
    decimal? AiQualityScore,
    DateTime CreatedAt
);

public record QualityInspectionDetailDto(
    Guid Id,
    Guid TenantId,
    Guid PurchaseOrderId,
    Guid? ShipmentId,
    Guid? BatchId,
    Guid InspectorUserId,
    string? InspectorName,
    InspectionType Type,
    InspectionStatus Status,
    DateTime? InspectionDate,
    string? Notes,
    string? PhotosJson,
    decimal? AiQualityScore,
    string? AiAnalysisJson,
    DateTime CreatedAt,
    IList<QualityReportDto> Reports
);

public record QualityReportDto(
    Guid Id,
    string Title,
    string? Summary,
    string? FileUrl,
    bool PassedOverallCheck,
    DateTime CreatedAt,
    IList<LabTestResultDto> LabTestResults
);

public record LabTestResultDto(
    Guid Id,
    string TestName,
    string? TestMethod,
    string? Parameter,
    string? ExpectedValue,
    string? ActualValue,
    string? Unit,
    bool Passed,
    string? LabName,
    DateTime? TestDate
);
