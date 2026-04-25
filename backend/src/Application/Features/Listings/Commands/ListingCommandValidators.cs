using FluentValidation;

namespace Rawnex.Application.Features.Listings.Commands;

public class CreateListingCommandValidator : AbstractValidator<CreateListingCommand>
{
    public CreateListingCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitOfMeasure).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.PriceCurrency).IsInEnum();
        RuleFor(x => x.PriceUnit).MaximumLength(50);
        RuleFor(x => x.MinOrderQuantity).GreaterThan(0).When(x => x.MinOrderQuantity.HasValue);
        RuleFor(x => x.Incoterm).IsInEnum();
        RuleFor(x => x.DeliveryLocation).MaximumLength(500);
        RuleFor(x => x.LeadTimeDays).GreaterThan(0).When(x => x.LeadTimeDays.HasValue);
    }
}

public class UpdateListingCommandValidator : AbstractValidator<UpdateListingCommand>
{
    public UpdateListingCommandValidator()
    {
        RuleFor(x => x.ListingId).NotEmpty();
        RuleFor(x => x.Title).MaximumLength(500).When(x => x.Title != null);
        RuleFor(x => x.Description).MaximumLength(4000).When(x => x.Description != null);
        RuleFor(x => x.Quantity).GreaterThan(0).When(x => x.Quantity.HasValue);
        RuleFor(x => x.Price).GreaterThan(0).When(x => x.Price.HasValue);
    }
}

public class ChangeListingStatusCommandValidator : AbstractValidator<ChangeListingStatusCommand>
{
    public ChangeListingStatusCommandValidator()
    {
        RuleFor(x => x.ListingId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class DeleteListingCommandValidator : AbstractValidator<DeleteListingCommand>
{
    public DeleteListingCommandValidator()
    {
        RuleFor(x => x.ListingId).NotEmpty();
    }
}
