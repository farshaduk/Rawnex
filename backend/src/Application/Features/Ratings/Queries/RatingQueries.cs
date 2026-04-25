using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Ratings.DTOs;

namespace Rawnex.Application.Features.Ratings.Queries;

public record GetCompanyRatingsQuery(Guid CompanyId, int PageNumber = 1, int PageSize = 20)
    : IRequest<Result<PaginatedList<RatingDto>>>;

public record GetOrderRatingQuery(Guid PurchaseOrderId, Guid ReviewerCompanyId) : IRequest<Result<RatingDto>>;
