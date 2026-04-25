using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Quality.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Quality.Commands;

public record CreateInspectionCommand(
    Guid PurchaseOrderId,
    Guid? ShipmentId,
    Guid? BatchId,
    InspectionType Type,
    DateTime? InspectionDate,
    string? Notes
) : IRequest<Result<QualityInspectionDto>>;

public record CompleteInspectionCommand(Guid InspectionId) : IRequest<Result>;

public record AddQualityReportCommand(
    Guid InspectionId,
    string Title,
    string? Summary,
    string? DetailedFindingsJson,
    string? FileUrl,
    bool PassedOverallCheck,
    List<CreateLabTestDto>? LabTests
) : IRequest<Result<QualityReportDto>>;

public record CreateLabTestDto(
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
