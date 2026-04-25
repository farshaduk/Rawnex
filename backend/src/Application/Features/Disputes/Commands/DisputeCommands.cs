using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Disputes.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Disputes.Commands;

public record FileDisputeCommand(
    Guid PurchaseOrderId,
    Guid FiledByCompanyId,
    Guid AgainstCompanyId,
    DisputeReason Reason,
    string Description,
    decimal? ClaimedAmount,
    Currency? ClaimedCurrency
) : IRequest<Result<DisputeDto>>;

public record AddDisputeEvidenceCommand(
    Guid DisputeId,
    string Title,
    string? Description,
    string FileUrl,
    string? MimeType,
    long FileSizeBytes
) : IRequest<Result<DisputeEvidenceDto>>;

public record ResolveDisputeCommand(
    Guid DisputeId,
    DisputeResolution Resolution,
    string? ResolutionNotes,
    decimal? ResolvedAmount
) : IRequest<Result>;
