using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class CommissionRule : BaseAuditableEntity
{
    public string Name { get; set; } = default!;
    public CommissionType Type { get; set; }
    public decimal Value { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinTransactionAmount { get; set; }
    public decimal? MaxTransactionAmount { get; set; }
    public Currency? Currency { get; set; }
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; }

    // Navigation
    public ProductCategory? Category { get; set; }
}
