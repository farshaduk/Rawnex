using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Ratings.DTOs;

public record RatingDto(
    Guid Id,
    Guid PurchaseOrderId,
    Guid ReviewerCompanyId,
    string? ReviewerCompanyName,
    Guid ReviewedCompanyId,
    string? ReviewedCompanyName,
    int OverallScore,
    int? QualityScore,
    int? DeliveryScore,
    int? CommunicationScore,
    int? ValueScore,
    string? Comment,
    bool IsPublic,
    string? ResponseComment,
    DateTime CreatedAt
);
