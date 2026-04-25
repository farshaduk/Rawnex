using FluentValidation;

namespace Rawnex.Application.Features.Auctions.Commands;

public class CreateAuctionCommandValidator : AbstractValidator<CreateAuctionCommand>
{
    public CreateAuctionCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitOfMeasure).NotEmpty().MaximumLength(50);
        RuleFor(x => x.StartingPrice).GreaterThan(0);
        RuleFor(x => x.ReservePrice).GreaterThan(0).When(x => x.ReservePrice.HasValue);
        RuleFor(x => x.PriceCurrency).IsInEnum();
        RuleFor(x => x.MinBidIncrement).GreaterThan(0);
        RuleFor(x => x.ScheduledStartAt).GreaterThan(DateTime.UtcNow).WithMessage("Start time must be in the future.");
        RuleFor(x => x.ScheduledEndAt).GreaterThan(x => x.ScheduledStartAt).WithMessage("End time must be after start time.");
    }
}

public class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand>
{
    public PlaceBidCommandValidator()
    {
        RuleFor(x => x.AuctionId).NotEmpty();
        RuleFor(x => x.BidderCompanyId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}

public class StartAuctionCommandValidator : AbstractValidator<StartAuctionCommand>
{
    public StartAuctionCommandValidator()
    {
        RuleFor(x => x.AuctionId).NotEmpty();
    }
}

public class EndAuctionCommandValidator : AbstractValidator<EndAuctionCommand>
{
    public EndAuctionCommandValidator()
    {
        RuleFor(x => x.AuctionId).NotEmpty();
    }
}

public class CancelAuctionCommandValidator : AbstractValidator<CancelAuctionCommand>
{
    public CancelAuctionCommandValidator()
    {
        RuleFor(x => x.AuctionId).NotEmpty();
    }
}
