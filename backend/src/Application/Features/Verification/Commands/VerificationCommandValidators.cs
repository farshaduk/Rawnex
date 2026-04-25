using FluentValidation;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Verification.Commands;

public class SubmitKycCommandValidator : AbstractValidator<SubmitKycCommand>
{
    public SubmitKycCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NationalId).MaximumLength(50);
        RuleFor(x => x.PassportNumber).MaximumLength(50);
        RuleFor(x => x.Nationality).MaximumLength(100);
        RuleFor(x => x.IdDocumentUrl).NotEmpty().MaximumLength(1000);
    }
}

public class ReviewKycCommandValidator : AbstractValidator<ReviewKycCommand>
{
    public ReviewKycCommandValidator()
    {
        RuleFor(x => x.KycId).NotEmpty();
        RuleFor(x => x.Decision).Must(d => d == VerificationStatus.Approved || d == VerificationStatus.Rejected)
            .WithMessage("Decision must be Approved or Rejected.");
        RuleFor(x => x.RejectionReason).NotEmpty().When(x => x.Decision == VerificationStatus.Rejected)
            .WithMessage("Rejection reason is required when rejecting.");
    }
}

public class SubmitKybCommandValidator : AbstractValidator<SubmitKybCommand>
{
    public SubmitKybCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.CompanyRegistrationDocUrl).NotEmpty().MaximumLength(1000);
    }
}

public class ReviewKybCommandValidator : AbstractValidator<ReviewKybCommand>
{
    public ReviewKybCommandValidator()
    {
        RuleFor(x => x.KybId).NotEmpty();
        RuleFor(x => x.Decision).Must(d => d == VerificationStatus.Approved || d == VerificationStatus.Rejected)
            .WithMessage("Decision must be Approved or Rejected.");
        RuleFor(x => x.RejectionReason).NotEmpty().When(x => x.Decision == VerificationStatus.Rejected);
    }
}
