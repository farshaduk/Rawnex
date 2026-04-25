using FluentValidation;

namespace Rawnex.Application.Features.Shipments.Commands;

public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId).NotEmpty();
        RuleFor(x => x.SellerCompanyId).NotEmpty();
        RuleFor(x => x.BuyerCompanyId).NotEmpty();
        RuleFor(x => x.TransportMode).IsInEnum();
        RuleFor(x => x.Incoterm).IsInEnum();
        RuleFor(x => x.CarrierName).MaximumLength(200);
        RuleFor(x => x.CarrierTrackingNumber).MaximumLength(100);
        RuleFor(x => x.ContainerNumber).MaximumLength(50);
        RuleFor(x => x.OriginCity).MaximumLength(100);
        RuleFor(x => x.OriginCountry).MaximumLength(100);
        RuleFor(x => x.DestinationCity).MaximumLength(100);
        RuleFor(x => x.DestinationCountry).MaximumLength(100);
        RuleFor(x => x.GrossWeightKg).GreaterThan(0).When(x => x.GrossWeightKg.HasValue);
        RuleFor(x => x.NumberOfPackages).GreaterThan(0).When(x => x.NumberOfPackages.HasValue);
        RuleFor(x => x.ShippingCost).GreaterThan(0).When(x => x.ShippingCost.HasValue);
    }
}

public class UpdateShipmentStatusCommandValidator : AbstractValidator<UpdateShipmentStatusCommand>
{
    public UpdateShipmentStatusCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Location).MaximumLength(500);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}

public class AddBatchToShipmentCommandValidator : AbstractValidator<AddBatchToShipmentCommand>
{
    public AddBatchToShipmentCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.BatchNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitOfMeasure).NotEmpty().MaximumLength(50);
    }
}

public class RequestFreightQuoteCommandValidator : AbstractValidator<RequestFreightQuoteCommand>
{
    public RequestFreightQuoteCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId).NotEmpty();
        RuleFor(x => x.CarrierName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TransportMode).IsInEnum();
        RuleFor(x => x.OriginCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.OriginCountry).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DestinationCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DestinationCountry).NotEmpty().MaximumLength(100);
        RuleFor(x => x.QuotedPrice).GreaterThan(0);
        RuleFor(x => x.Currency).IsInEnum();
    }
}

public class SelectFreightQuoteCommandValidator : AbstractValidator<SelectFreightQuoteCommand>
{
    public SelectFreightQuoteCommandValidator()
    {
        RuleFor(x => x.FreightQuoteId).NotEmpty();
    }
}
