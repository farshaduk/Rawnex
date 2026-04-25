using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Shipment : BaseAuditableEntity, IAggregateRoot, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid SellerCompanyId { get; set; }
    public Guid BuyerCompanyId { get; set; }
    public string ShipmentNumber { get; set; } = default!;
    public ShipmentStatus Status { get; set; }
    public TransportMode TransportMode { get; set; }
    public Incoterm Incoterm { get; set; }

    // Carrier info
    public string? CarrierName { get; set; }
    public string? CarrierTrackingNumber { get; set; }
    public string? BillOfLadingNumber { get; set; }
    public string? BillOfLadingUrl { get; set; }
    public string? ContainerNumber { get; set; }
    public string? SealNumber { get; set; }
    public string? ContainerSealPhotoUrl { get; set; }

    // Origin
    public string? OriginAddress { get; set; }
    public string? OriginCity { get; set; }
    public string? OriginCountry { get; set; }

    // Destination
    public string? DestinationAddress { get; set; }
    public string? DestinationCity { get; set; }
    public string? DestinationCountry { get; set; }

    // Weight & dimensions
    public decimal? GrossWeightKg { get; set; }
    public decimal? NetWeightKg { get; set; }
    public int? NumberOfPackages { get; set; }

    // Timeline
    public DateTime? EstimatedDepartureDate { get; set; }
    public DateTime? ActualDepartureDate { get; set; }
    public DateTime? EstimatedArrivalDate { get; set; }
    public DateTime? ActualArrivalDate { get; set; }
    public DateTime? DeliveredAt { get; set; }

    // Costs
    public decimal? ShippingCost { get; set; }
    public decimal? InsuranceCost { get; set; }
    public Currency? CostCurrency { get; set; }

    // Carbon footprint
    public decimal? CarbonFootprintKg { get; set; }

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public Company SellerCompany { get; set; } = default!;
    public Company BuyerCompany { get; set; } = default!;
    public ICollection<ShipmentTracking> TrackingEvents { get; set; } = new List<ShipmentTracking>();
    public ICollection<Batch> Batches { get; set; } = new List<Batch>();
}
