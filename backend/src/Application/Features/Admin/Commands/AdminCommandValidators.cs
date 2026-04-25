using FluentValidation;

namespace Rawnex.Application.Features.Admin.Commands;

public class CreateCommissionRuleCommandValidator : AbstractValidator<CreateCommissionRuleCommand>
{
    public CreateCommissionRuleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Value).GreaterThan(0);
        RuleFor(x => x.MinTransactionAmount).GreaterThanOrEqualTo(0).When(x => x.MinTransactionAmount.HasValue);
        RuleFor(x => x.MaxTransactionAmount).GreaterThan(x => x.MinTransactionAmount ?? 0)
            .When(x => x.MaxTransactionAmount.HasValue && x.MinTransactionAmount.HasValue);
        RuleFor(x => x.Currency).IsInEnum().When(x => x.Currency.HasValue);
    }
}

public class UpdateCommissionRuleCommandValidator : AbstractValidator<UpdateCommissionRuleCommand>
{
    public UpdateCommissionRuleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Value).GreaterThan(0);
    }
}

public class DeleteCommissionRuleCommandValidator : AbstractValidator<DeleteCommissionRuleCommand>
{
    public DeleteCommissionRuleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class UpsertFeatureFlagCommandValidator : AbstractValidator<UpsertFeatureFlagCommand>
{
    public UpsertFeatureFlagCommandValidator()
    {
        RuleFor(x => x.Key).NotEmpty().MaximumLength(100).Matches(@"^[a-z0-9_.-]+$").WithMessage("Key must be lowercase alphanumeric with dots, hyphens, or underscores.");
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.RolloutPercentage).InclusiveBetween(0, 100).When(x => x.RolloutPercentage.HasValue);
    }
}

public class DeleteFeatureFlagCommandValidator : AbstractValidator<DeleteFeatureFlagCommand>
{
    public DeleteFeatureFlagCommandValidator()
    {
        RuleFor(x => x.Key).NotEmpty();
    }
}

public class UpdateTenantStatusCommandValidator : AbstractValidator<UpdateTenantStatusCommand>
{
    public UpdateTenantStatusCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class UpdateTenantPlanCommandValidator : AbstractValidator<UpdateTenantPlanCommand>
{
    public UpdateTenantPlanCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Plan).IsInEnum();
    }
}

public class MarkBillingPaidCommandValidator : AbstractValidator<MarkBillingPaidCommand>
{
    public MarkBillingPaidCommandValidator()
    {
        RuleFor(x => x.BillingId).NotEmpty();
    }
}
