using FluentValidation;

namespace Rawnex.Application.Features.Payments.Commands;

public class CreateEscrowAccountCommandValidator : AbstractValidator<CreateEscrowAccountCommand>
{
    public CreateEscrowAccountCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId).NotEmpty();
        RuleFor(x => x.BuyerCompanyId).NotEmpty();
        RuleFor(x => x.SellerCompanyId).NotEmpty();
        RuleFor(x => x.TotalAmount).GreaterThan(0);
        RuleFor(x => x.Currency).IsInEnum();
        RuleForEach(x => x.Milestones).ChildRules(m =>
        {
            m.RuleFor(i => i.Description).NotEmpty().MaximumLength(300);
            m.RuleFor(i => i.ReleasePercentage).GreaterThan(0).LessThanOrEqualTo(100);
            m.RuleFor(i => i.Type).IsInEnum();
        }).When(x => x.Milestones != null);
    }
}

public class FundEscrowCommandValidator : AbstractValidator<FundEscrowCommand>
{
    public FundEscrowCommandValidator()
    {
        RuleFor(x => x.EscrowAccountId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public class CompleteMilestoneCommandValidator : AbstractValidator<CompleteMilestoneCommand>
{
    public CompleteMilestoneCommandValidator()
    {
        RuleFor(x => x.EscrowAccountId).NotEmpty();
        RuleFor(x => x.MilestoneId).NotEmpty();
    }
}

public class RecordPaymentCommandValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentCommandValidator()
    {
        RuleFor(x => x.PayerCompanyId).NotEmpty();
        RuleFor(x => x.PayeeCompanyId).NotEmpty();
        RuleFor(x => x.Method).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).IsInEnum();
    }
}

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId).NotEmpty();
        RuleFor(x => x.IssuerCompanyId).NotEmpty();
        RuleFor(x => x.RecipientCompanyId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.DueDate).GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.");
        RuleFor(x => x.Currency).IsInEnum();
        RuleFor(x => x.Items).NotEmpty().WithMessage("Invoice must have at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Description).NotEmpty().MaximumLength(500);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}

public class MarkInvoicePaidCommandValidator : AbstractValidator<MarkInvoicePaidCommand>
{
    public MarkInvoicePaidCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
    }
}
