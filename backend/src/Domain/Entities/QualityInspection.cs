using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class QualityInspection : BaseAuditableEntity, IAggregateRoot, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid? ShipmentId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid InspectorUserId { get; set; }
    public InspectionType Type { get; set; }
    public InspectionStatus Status { get; set; }
    public DateTime? InspectionDate { get; set; }
    public string? Notes { get; set; }
    public string? PhotosJson { get; set; }
    public decimal? AiQualityScore { get; set; }
    public string? AiAnalysisJson { get; set; }

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public Shipment? Shipment { get; set; }
    public Batch? Batch { get; set; }
    public ApplicationUser InspectorUser { get; set; } = default!;
    public ICollection<QualityReport> Reports { get; set; } = new List<QualityReport>();
}
