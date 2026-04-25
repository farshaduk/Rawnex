using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Rfqs.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Rfqs.Queries;

public record GetRfqByIdQuery(Guid RfqId) : IRequest<Result<RfqDetailDto>>;

public record SearchRfqsQuery(
    RfqStatus? Status,
    Guid? BuyerCompanyId,
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedList<RfqDto>>>;

public record GetMyRfqResponsesQuery(
    Guid SellerCompanyId,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedList<RfqResponseDto>>>;
