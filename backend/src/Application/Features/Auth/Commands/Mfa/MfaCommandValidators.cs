using FluentValidation;

namespace Rawnex.Application.Features.Auth.Commands.Mfa;

public class VerifyMfaSetupCommandValidator : AbstractValidator<VerifyMfaSetupCommand>
{
    public VerifyMfaSetupCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().Length(6).Matches(@"^\d{6}$").WithMessage("Code must be a 6-digit number.");
    }
}

public class DisableMfaCommandValidator : AbstractValidator<DisableMfaCommand>
{
    public DisableMfaCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().Length(6).Matches(@"^\d{6}$").WithMessage("Code must be a 6-digit number.");
    }
}

public class VerifyMfaLoginCommandValidator : AbstractValidator<VerifyMfaLoginCommand>
{
    public VerifyMfaLoginCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
    }
}
