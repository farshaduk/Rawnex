using FluentValidation;

namespace Rawnex.Application.Features.Contracts.Commands;

public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(x => x.BuyerCompanyId).NotEmpty();
        RuleFor(x => x.SellerCompanyId).NotEmpty();
        RuleFor(x => x.BuyerCompanyId).NotEqual(x => x.SellerCompanyId).WithMessage("Buyer and seller cannot be the same.");
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.TotalValue).GreaterThan(0);
        RuleFor(x => x.Currency).IsInEnum();
        RuleFor(x => x.Incoterm).IsInEnum();
        RuleFor(x => x.EffectiveDate).NotEmpty();
        RuleFor(x => x.ExpirationDate).GreaterThan(x => x.EffectiveDate).WithMessage("Expiration must be after effective date.");
        RuleForEach(x => x.Clauses).ChildRules(clause =>
        {
            clause.RuleFor(c => c.Title).NotEmpty().MaximumLength(300);
            clause.RuleFor(c => c.Content).NotEmpty().MaximumLength(8000);
        }).When(x => x.Clauses != null);
    }
}

public class SignContractCommandValidator : AbstractValidator<SignContractCommand>
{
    public SignContractCommandValidator()
    {
        RuleFor(x => x.ContractId).NotEmpty();
        RuleFor(x => x.SignerCompanyId).NotEmpty();
        RuleFor(x => x.SignerName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SignerRole).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SignatureHash).NotEmpty().MaximumLength(500);
    }
}

public class TerminateContractCommandValidator : AbstractValidator<TerminateContractCommand>
{
    public TerminateContractCommandValidator()
    {
        RuleFor(x => x.ContractId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(2000);
    }
}
