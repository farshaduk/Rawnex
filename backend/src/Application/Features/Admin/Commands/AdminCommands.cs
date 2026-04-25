using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Admin.Commands;

// Commission Rules
public record CreateCommissionRuleCommand(
    string Name,
    CommissionType Type,
    decimal Value,
    Guid? CategoryId,
    decimal? MinTransactionAmount,
    decimal? MaxTransactionAmount,
    Currency? Currency,
    int Priority
) : IRequest<Result<Guid>>;

public record UpdateCommissionRuleCommand(
    Guid Id,
    string Name,
    CommissionType Type,
    decimal Value,
    Guid? CategoryId,
    decimal? MinTransactionAmount,
    decimal? MaxTransactionAmount,
    Currency? Currency,
    int Priority,
    bool IsActive
) : IRequest<Result>;

public record DeleteCommissionRuleCommand(Guid Id) : IRequest<Result>;

// Feature Flags
public record UpsertFeatureFlagCommand(
    string Key,
    string? Description,
    bool IsEnabled,
    decimal? RolloutPercentage,
    string? TargetTenantsJson,
    string? TargetRolesJson
) : IRequest<Result<Guid>>;

public record DeleteFeatureFlagCommand(string Key) : IRequest<Result>;

// Tenants
public record UpdateTenantStatusCommand(Guid TenantId, TenantStatus Status) : IRequest<Result>;
public record UpdateTenantPlanCommand(Guid TenantId, TenantPlan Plan) : IRequest<Result>;

// Platform Billing
public record MarkBillingPaidCommand(Guid BillingId) : IRequest<Result>;
