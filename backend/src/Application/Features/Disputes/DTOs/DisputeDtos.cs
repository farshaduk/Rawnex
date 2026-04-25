using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Disputes.DTOs;

public record DisputeDto(
    Guid Id,
    string DisputeNumber,
    Guid PurchaseOrderId,
    Guid FiledByCompanyId,
    string? FiledByCompanyName,
    Guid AgainstCompanyId,
    string? AgainstCompanyName,
    DisputeStatus Status,
    DisputeReason Reason,
    decimal? ClaimedAmount,
    Currency? ClaimedCurrency,
    DateTime CreatedAt
);

public record DisputeDetailDto(
    Guid Id,
    Guid TenantId,
    string DisputeNumber,
    Guid PurchaseOrderId,
    Guid FiledByCompanyId,
    string? FiledByCompanyName,
    Guid FiledByUserId,
    Guid AgainstCompanyId,
    string? AgainstCompanyName,
    DisputeStatus Status,
    DisputeReason Reason,
    string Description,
    decimal? ClaimedAmount,
    Currency? ClaimedCurrency,
    DisputeResolution? Resolution,
    string? ResolutionNotes,
    decimal? ResolvedAmount,
    DateTime? ResolvedAt,
    DateTime CreatedAt,
    IList<DisputeEvidenceDto> Evidence
);

public record DisputeEvidenceDto(
    Guid Id,
    Guid UploadedByUserId,
    string Title,
    string? Description,
    string FileUrl,
    string? MimeType,
    long FileSizeBytes,
    DateTime CreatedAt
);
