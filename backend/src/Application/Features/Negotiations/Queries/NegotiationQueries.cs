using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Negotiations.DTOs;

namespace Rawnex.Application.Features.Negotiations.Queries;

public record GetNegotiationByIdQuery(Guid NegotiationId) : IRequest<Result<NegotiationDetailDto>>;

public record GetMyNegotiationsQuery(
    Guid CompanyId,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedList<NegotiationDto>>>;
