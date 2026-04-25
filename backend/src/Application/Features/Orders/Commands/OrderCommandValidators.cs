using FluentValidation;

namespace Rawnex.Application.Features.Orders.Commands;

public class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.BuyerCompanyId).NotEmpty();
        RuleFor(x => x.SellerCompanyId).NotEmpty();
        RuleFor(x => x.BuyerCompanyId).NotEqual(x => x.SellerCompanyId).WithMessage("Buyer and seller cannot be the same.");
        RuleFor(x => x.Incoterm).IsInEnum();
        RuleFor(x => x.DeliveryLocation).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Currency).IsInEnum();
        RuleFor(x => x.Items).NotEmpty().WithMessage("Order must have at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.ProductName).NotEmpty().MaximumLength(300);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.UnitOfMeasure).NotEmpty().MaximumLength(50);
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}

public class ConfirmOrderCommandValidator : AbstractValidator<ConfirmOrderCommand>
{
    public ConfirmOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(2000);
    }
}

public class ChangeOrderStatusCommandValidator : AbstractValidator<ChangeOrderStatusCommand>
{
    public ChangeOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.NewStatus).IsInEnum();
    }
}

public class ApproveOrderStepCommandValidator : AbstractValidator<ApproveOrderStepCommand>
{
    public ApproveOrderStepCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.ApprovalId).NotEmpty();
    }
}
