using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Admin.Commands;

// -- Commission Rules --

public class CreateCommissionRuleCommandHandler : IRequestHandler<CreateCommissionRuleCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;

    public CreateCommissionRuleCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateCommissionRuleCommand request, CancellationToken ct)
    {
        var rule = new CommissionRule
        {
            Name = request.Name,
            Type = request.Type,
            Value = request.Value,
            CategoryId = request.CategoryId,
            MinTransactionAmount = request.MinTransactionAmount,
            MaxTransactionAmount = request.MaxTransactionAmount,
            Currency = request.Currency,
            Priority = request.Priority,
            IsActive = true
        };

        _db.CommissionRules.Add(rule);
        await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(rule.Id);
    }
}

public class UpdateCommissionRuleCommandHandler : IRequestHandler<UpdateCommissionRuleCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateCommissionRuleCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateCommissionRuleCommand request, CancellationToken ct)
    {
        var rule = await _db.CommissionRules.FindAsync([request.Id], ct)
                   ?? throw new NotFoundException(nameof(CommissionRule), request.Id);

        rule.Name = request.Name;
        rule.Type = request.Type;
        rule.Value = request.Value;
        rule.CategoryId = request.CategoryId;
        rule.MinTransactionAmount = request.MinTransactionAmount;
        rule.MaxTransactionAmount = request.MaxTransactionAmount;
        rule.Currency = request.Currency;
        rule.Priority = request.Priority;
        rule.IsActive = request.IsActive;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class DeleteCommissionRuleCommandHandler : IRequestHandler<DeleteCommissionRuleCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteCommissionRuleCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteCommissionRuleCommand request, CancellationToken ct)
    {
        var rule = await _db.CommissionRules.FindAsync([request.Id], ct)
                   ?? throw new NotFoundException(nameof(CommissionRule), request.Id);

        _db.CommissionRules.Remove(rule);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// -- Feature Flags --

public class UpsertFeatureFlagCommandHandler : IRequestHandler<UpsertFeatureFlagCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;

    public UpsertFeatureFlagCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(UpsertFeatureFlagCommand request, CancellationToken ct)
    {
        var flag = await _db.FeatureFlags.FirstOrDefaultAsync(f => f.Key == request.Key, ct);

        if (flag is null)
        {
            flag = new FeatureFlag { Key = request.Key };
            _db.FeatureFlags.Add(flag);
        }

        flag.Description = request.Description;
        flag.IsEnabled = request.IsEnabled;
        flag.RolloutPercentage = request.RolloutPercentage;
        flag.TargetTenantsJson = request.TargetTenantsJson;
        flag.TargetRolesJson = request.TargetRolesJson;

        await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(flag.Id);
    }
}

public class DeleteFeatureFlagCommandHandler : IRequestHandler<DeleteFeatureFlagCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteFeatureFlagCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteFeatureFlagCommand request, CancellationToken ct)
    {
        var flag = await _db.FeatureFlags.FirstOrDefaultAsync(f => f.Key == request.Key, ct)
                   ?? throw new NotFoundException(nameof(FeatureFlag), request.Key);

        _db.FeatureFlags.Remove(flag);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// -- Tenants --

public class UpdateTenantStatusCommandHandler : IRequestHandler<UpdateTenantStatusCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateTenantStatusCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateTenantStatusCommand request, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FindAsync([request.TenantId], ct)
                     ?? throw new NotFoundException(nameof(Tenant), request.TenantId);

        tenant.Status = request.Status;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class UpdateTenantPlanCommandHandler : IRequestHandler<UpdateTenantPlanCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateTenantPlanCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateTenantPlanCommand request, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FindAsync([request.TenantId], ct)
                     ?? throw new NotFoundException(nameof(Tenant), request.TenantId);

        tenant.Plan = request.Plan;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// -- Platform Billing --

public class MarkBillingPaidCommandHandler : IRequestHandler<MarkBillingPaidCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public MarkBillingPaidCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(MarkBillingPaidCommand request, CancellationToken ct)
    {
        var billing = await _db.PlatformBillings.FindAsync([request.BillingId], ct)
                      ?? throw new NotFoundException(nameof(PlatformBilling), request.BillingId);

        billing.IsPaid = true;
        billing.PaidAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
