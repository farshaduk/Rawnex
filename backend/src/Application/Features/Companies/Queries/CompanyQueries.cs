using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Companies.DTOs;

namespace Rawnex.Application.Features.Companies.Queries;

public record GetCompanyByIdQuery(Guid CompanyId) : IRequest<Result<CompanyDetailDto>>;

public record GetMyCompaniesQuery : IRequest<Result<IList<CompanyDto>>>;

public record SearchCompaniesQuery(
    string? SearchTerm,
    string? Country,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedList<CompanyDto>>>;
