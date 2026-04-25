using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Contracts.DTOs;

namespace Rawnex.Application.Features.Contracts.Queries;

public record GetContractByIdQuery(Guid ContractId) : IRequest<Result<ContractDetailDto>>;

public record GetCompanyContractsQuery(
    Guid CompanyId,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedList<ContractDto>>>;
