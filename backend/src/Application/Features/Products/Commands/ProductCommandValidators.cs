using FluentValidation;

namespace Rawnex.Application.Features.Products.Commands;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.NameFa).MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.Sku).MaximumLength(50);
        RuleFor(x => x.CasNumber).MaximumLength(20);
        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0).When(x => x.BasePrice.HasValue);
    }
}

public class CreateProductCategoryCommandValidator : AbstractValidator<CreateProductCategoryCommand>
{
    public CreateProductCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameFa).MaximumLength(200);
    }
}
