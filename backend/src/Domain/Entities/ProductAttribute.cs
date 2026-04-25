using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class ProductAttribute : BaseEntity
{
    public Guid ProductId { get; set; }
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string? Unit { get; set; }
    public int SortOrder { get; set; }

    // Navigation
    public Product Product { get; set; } = default!;
}
