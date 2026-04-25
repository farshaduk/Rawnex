using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string Description { get; set; } = default!;
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public Currency Currency { get; set; }

    // Navigation
    public Invoice Invoice { get; set; } = default!;
}
