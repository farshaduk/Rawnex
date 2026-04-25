using FluentValidation;

namespace Rawnex.Application.Features.Negotiations.Commands;

public class StartNegotiationCommandValidator : AbstractValidator<StartNegotiationCommand>
{
    public StartNegotiationCommandValidator()
    {
        RuleFor(x => x.BuyerCompanyId).NotEmpty();
        RuleFor(x => x.SellerCompanyId).NotEmpty();
        RuleFor(x => x.BuyerCompanyId).NotEqual(x => x.SellerCompanyId).WithMessage("Buyer and seller cannot be the same company.");
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(500);
        RuleFor(x => x.InitialMessage).NotEmpty().MaximumLength(4000);
    }
}

public class SendNegotiationMessageCommandValidator : AbstractValidator<SendNegotiationMessageCommand>
{
    public SendNegotiationMessageCommandValidator()
    {
        RuleFor(x => x.NegotiationId).NotEmpty();
        RuleFor(x => x.SenderCompanyId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(4000);
    }
}

public class AcceptNegotiationCommandValidator : AbstractValidator<AcceptNegotiationCommand>
{
    public AcceptNegotiationCommandValidator()
    {
        RuleFor(x => x.NegotiationId).NotEmpty();
        RuleFor(x => x.AgreedPrice).GreaterThan(0);
        RuleFor(x => x.AgreedCurrency).IsInEnum();
        RuleFor(x => x.AgreedQuantity).GreaterThan(0);
        RuleFor(x => x.AgreedIncoterm).IsInEnum();
    }
}

public class RejectNegotiationCommandValidator : AbstractValidator<RejectNegotiationCommand>
{
    public RejectNegotiationCommandValidator()
    {
        RuleFor(x => x.NegotiationId).NotEmpty();
    }
}
