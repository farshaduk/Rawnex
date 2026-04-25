using FluentValidation;

namespace Rawnex.Application.Features.Disputes.Commands;

public class FileDisputeCommandValidator : AbstractValidator<FileDisputeCommand>
{
    public FileDisputeCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId).NotEmpty();
        RuleFor(x => x.FiledByCompanyId).NotEmpty();
        RuleFor(x => x.AgainstCompanyId).NotEmpty();
        RuleFor(x => x.FiledByCompanyId).NotEqual(x => x.AgainstCompanyId).WithMessage("Cannot file a dispute against yourself.");
        RuleFor(x => x.Reason).IsInEnum();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.ClaimedAmount).GreaterThan(0).When(x => x.ClaimedAmount.HasValue);
        RuleFor(x => x.ClaimedCurrency).IsInEnum().When(x => x.ClaimedCurrency.HasValue);
    }
}

public class AddDisputeEvidenceCommandValidator : AbstractValidator<AddDisputeEvidenceCommand>
{
    public AddDisputeEvidenceCommandValidator()
    {
        RuleFor(x => x.DisputeId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.FileUrl).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.MimeType).MaximumLength(100);
    }
}

public class ResolveDisputeCommandValidator : AbstractValidator<ResolveDisputeCommand>
{
    public ResolveDisputeCommandValidator()
    {
        RuleFor(x => x.DisputeId).NotEmpty();
        RuleFor(x => x.Resolution).IsInEnum();
        RuleFor(x => x.ResolutionNotes).MaximumLength(4000);
        RuleFor(x => x.ResolvedAmount).GreaterThan(0).When(x => x.ResolvedAmount.HasValue);
    }
}
