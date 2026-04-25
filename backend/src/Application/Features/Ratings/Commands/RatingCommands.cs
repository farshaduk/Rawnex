using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Ratings.DTOs;

namespace Rawnex.Application.Features.Ratings.Commands;

public record SubmitRatingCommand(
    Guid PurchaseOrderId,
    Guid ReviewerCompanyId,
    Guid ReviewedCompanyId,
    int OverallScore,
    int? QualityScore,
    int? DeliveryScore,
    int? CommunicationScore,
    int? ValueScore,
    string? Comment,
    bool IsPublic = true
) : IRequest<Result<RatingDto>>;

public record RespondToRatingCommand(
    Guid RatingId,
    string ResponseComment
) : IRequest<Result>;
