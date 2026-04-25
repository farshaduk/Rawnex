using FluentValidation;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Workflows.Commands;

public class InitiateApprovalWorkflowCommandValidator : AbstractValidator<InitiateApprovalWorkflowCommand>
{
    public InitiateApprovalWorkflowCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId).NotEmpty();
        RuleFor(x => x.Steps).NotEmpty().WithMessage("At least one approval step is required.");
        RuleForEach(x => x.Steps).ChildRules(step =>
        {
            step.RuleFor(s => s.StepName).NotEmpty().MaximumLength(200);
            step.RuleFor(s => s.StepOrder).GreaterThan(0);
        });
    }
}

public class DecideApprovalStepCommandValidator : AbstractValidator<DecideApprovalStepCommand>
{
    public DecideApprovalStepCommandValidator()
    {
        RuleFor(x => x.ApprovalId).NotEmpty();
        RuleFor(x => x.Decision).Must(d => d == ApprovalStatus.Approved || d == ApprovalStatus.Rejected)
            .WithMessage("Decision must be Approved or Rejected.");
        RuleFor(x => x.Comments).MaximumLength(2000);
        RuleFor(x => x.Comments).NotEmpty().When(x => x.Decision == ApprovalStatus.Rejected)
            .WithMessage("Comments are required when rejecting.");
    }
}
