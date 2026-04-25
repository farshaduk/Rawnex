using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Verification.DTOs;

public record KycVerificationDto(
    Guid Id,
    Guid UserId,
    VerificationStatus Status,
    string? FullName,
    string? Nationality,
    string? RejectionReason,
    DateTime? ReviewedAt,
    DateTime CreatedAt);

public record KybVerificationDto(
    Guid Id,
    Guid CompanyId,
    VerificationStatus Status,
    string? RejectionReason,
    DateTime? ReviewedAt,
    DateTime CreatedAt);
