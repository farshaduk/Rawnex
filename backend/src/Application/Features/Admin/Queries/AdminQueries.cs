using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Admin.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Admin.Queries;

public record GetCommissionRulesQuery : IRequest<Result<List<CommissionRuleDto>>>;

public record GetFeatureFlagsQuery : IRequest<Result<List<FeatureFlagDto>>>;

public record GetTenantsQuery(int PageNumber = 1, int PageSize = 20, TenantStatus? Status = null)
    : IRequest<Result<PaginatedList<TenantDto>>>;

public record GetTenantByIdQuery(Guid TenantId) : IRequest<Result<TenantDto>>;

public record GetPlatformBillingsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? TenantId = null,
    bool? IsPaid = null
) : IRequest<Result<PaginatedList<PlatformBillingDto>>>;
