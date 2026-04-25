using FluentValidation;

namespace Rawnex.Application.Features.Quality.Commands;

public class CreateInspectionCommandValidator : AbstractValidator<CreateInspectionCommand>
{
    public CreateInspectionCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
    }
}

public class CompleteInspectionCommandValidator : AbstractValidator<CompleteInspectionCommand>
{
    public CompleteInspectionCommandValidator()
    {
        RuleFor(x => x.InspectionId).NotEmpty();
    }
}

public class AddQualityReportCommandValidator : AbstractValidator<AddQualityReportCommand>
{
    public AddQualityReportCommandValidator()
    {
        RuleFor(x => x.InspectionId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Summary).MaximumLength(4000);
        RuleForEach(x => x.LabTests).ChildRules(lt =>
        {
            lt.RuleFor(t => t.TestName).NotEmpty().MaximumLength(200);
            lt.RuleFor(t => t.TestMethod).MaximumLength(200);
            lt.RuleFor(t => t.Parameter).MaximumLength(200);
        }).When(x => x.LabTests != null);
    }
}
