using FluentValidation;

namespace Rawnex.Application.Features.Rfqs.Commands;

public class CreateRfqCommandValidator : AbstractValidator<CreateRfqCommand>
{
    public CreateRfqCommandValidator()
    {
        RuleFor(x => x.BuyerCompanyId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.Visibility).IsInEnum();
        RuleFor(x => x.MaterialName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.RequiredQuantity).GreaterThan(0);
        RuleFor(x => x.UnitOfMeasure).NotEmpty().MaximumLength(50);
        RuleFor(x => x.BudgetMin).GreaterThanOrEqualTo(0).When(x => x.BudgetMin.HasValue);
        RuleFor(x => x.BudgetMax).GreaterThan(0).When(x => x.BudgetMax.HasValue);
        RuleFor(x => x.BudgetMax).GreaterThanOrEqualTo(x => x.BudgetMin ?? 0).When(x => x.BudgetMax.HasValue && x.BudgetMin.HasValue);
        RuleFor(x => x.BudgetCurrency).IsInEnum();
        RuleFor(x => x.PreferredIncoterm).IsInEnum().When(x => x.PreferredIncoterm.HasValue);
        RuleFor(x => x.DeliveryLocation).MaximumLength(500);
        RuleFor(x => x.ResponseDeadline).GreaterThan(DateTime.UtcNow).WithMessage("Response deadline must be in the future.");
    }
}

public class SubmitRfqResponseCommandValidator : AbstractValidator<SubmitRfqResponseCommand>
{
    public SubmitRfqResponseCommandValidator()
    {
        RuleFor(x => x.RfqId).NotEmpty();
        RuleFor(x => x.SellerCompanyId).NotEmpty();
        RuleFor(x => x.ProposedPrice).GreaterThan(0);
        RuleFor(x => x.PriceCurrency).IsInEnum();
        RuleFor(x => x.ProposedQuantity).GreaterThan(0);
        RuleFor(x => x.UnitOfMeasure).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LeadTimeDays).GreaterThan(0).When(x => x.LeadTimeDays.HasValue);
        RuleFor(x => x.Notes).MaximumLength(4000);
    }
}

public class AwardRfqCommandValidator : AbstractValidator<AwardRfqCommand>
{
    public AwardRfqCommandValidator()
    {
        RuleFor(x => x.RfqId).NotEmpty();
        RuleFor(x => x.AwardedToCompanyId).NotEmpty();
    }
}

public class CancelRfqCommandValidator : AbstractValidator<CancelRfqCommand>
{
    public CancelRfqCommandValidator()
    {
        RuleFor(x => x.RfqId).NotEmpty();
    }
}
