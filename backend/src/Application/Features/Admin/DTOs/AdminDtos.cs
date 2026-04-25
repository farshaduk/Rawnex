using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Admin.DTOs;

public record CommissionRuleDto(
    Guid Id,
    string Name,
    CommissionType Type,
    decimal Value,
    Guid? CategoryId,
    string? CategoryName,
    decimal? MinTransactionAmount,
    decimal? MaxTransactionAmount,
    Currency? Currency,
    bool IsActive,
    int Priority
);

public record FeatureFlagDto(
    Guid Id,
    string Key,
    string? Description,
    bool IsEnabled,
    decimal? RolloutPercentage,
    string? TargetTenantsJson,
    string? TargetRolesJson,
    bool IsEnabledForAll
);

public record PlatformBillingDto(
    Guid Id,
    Guid TenantId,
    string TenantName,
    Guid? PurchaseOrderId,
    string BillingReference,
    decimal Amount,
    Currency Currency,
    CommissionType CommissionType,
    decimal CommissionRate,
    bool IsPaid,
    DateTime? PaidAt,
    string? InvoiceUrl,
    DateTime CreatedAt
);

public record TenantDto(
    Guid Id,
    string Name,
    string Subdomain,
    string? LogoUrl,
    TenantStatus Status,
    TenantPlan Plan,
    string? ContactEmail,
    string? ContactPhone,
    DateTime? SubscriptionExpiresAt,
    DateTime CreatedAt
);
