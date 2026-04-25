using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class ContractClause : BaseEntity
{
    public Guid ContractId { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public int SortOrder { get; set; }
    public bool IsStandard { get; set; }

    // Navigation
    public Contract Contract { get; set; } = default!;
}
