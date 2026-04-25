using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Admin.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Admin.Queries;

public class GetCommissionRulesQueryHandler : IRequestHandler<GetCommissionRulesQuery, Result<List<CommissionRuleDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetCommissionRulesQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<CommissionRuleDto>>> Handle(GetCommissionRulesQuery request, CancellationToken ct)
    {
        var rules = await _db.CommissionRules.AsNoTracking()
            .Include(r => r.Category)
            .OrderBy(r => r.Priority)
            .Select(r => new CommissionRuleDto(
                r.Id, r.Name, r.Type, r.Value, r.CategoryId,
                r.Category != null ? r.Category.Name : null,
                r.MinTransactionAmount, r.MaxTransactionAmount,
                r.Currency, r.IsActive, r.Priority))
            .ToListAsync(ct);

        return Result<List<CommissionRuleDto>>.Success(rules);
    }
}

public class GetFeatureFlagsQueryHandler : IRequestHandler<GetFeatureFlagsQuery, Result<List<FeatureFlagDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetFeatureFlagsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<FeatureFlagDto>>> Handle(GetFeatureFlagsQuery request, CancellationToken ct)
    {
        var flags = await _db.FeatureFlags.AsNoTracking()
            .OrderBy(f => f.Key)
            .Select(f => new FeatureFlagDto(
                f.Id, f.Key, f.Description, f.IsEnabled,
                f.RolloutPercentage, f.TargetTenantsJson,
                f.TargetRolesJson, f.IsEnabledForAll))
            .ToListAsync(ct);

        return Result<List<FeatureFlagDto>>.Success(flags);
    }
}

public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, Result<PaginatedList<TenantDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetTenantsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<TenantDto>>> Handle(GetTenantsQuery request, CancellationToken ct)
    {
        var query = _db.Tenants.AsNoTracking().AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        var result = await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TenantDto(
                t.Id, t.Name, t.Subdomain, t.LogoUrl, t.Status,
                t.Plan, t.ContactEmail, t.ContactPhone,
                t.SubscriptionExpiresAt, t.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<TenantDto>>.Success(result);
    }
}

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, Result<TenantDto>>
{
    private readonly IApplicationDbContext _db;

    public GetTenantByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<TenantDto>> Handle(GetTenantByIdQuery request, CancellationToken ct)
    {
        var tenant = await _db.Tenants.AsNoTracking()
            .Where(t => t.Id == request.TenantId)
            .Select(t => new TenantDto(
                t.Id, t.Name, t.Subdomain, t.LogoUrl, t.Status,
                t.Plan, t.ContactEmail, t.ContactPhone,
                t.SubscriptionExpiresAt, t.CreatedAt))
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(nameof(Tenant), request.TenantId);

        return Result<TenantDto>.Success(tenant);
    }
}

public class GetPlatformBillingsQueryHandler : IRequestHandler<GetPlatformBillingsQuery, Result<PaginatedList<PlatformBillingDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetPlatformBillingsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<PlatformBillingDto>>> Handle(GetPlatformBillingsQuery request, CancellationToken ct)
    {
        var query = _db.PlatformBillings.AsNoTracking()
            .Include(b => b.Tenant)
            .AsQueryable();

        if (request.TenantId.HasValue)
            query = query.Where(b => b.TenantId == request.TenantId.Value);

        if (request.IsPaid.HasValue)
            query = query.Where(b => b.IsPaid == request.IsPaid.Value);

        var result = await query
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new PlatformBillingDto(
                b.Id, b.TenantId, b.Tenant.Name, b.PurchaseOrderId,
                b.BillingReference, b.Amount, b.Currency,
                b.CommissionType, b.CommissionRate, b.IsPaid,
                b.PaidAt, b.InvoiceUrl, b.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<PlatformBillingDto>>.Success(result);
    }
}
