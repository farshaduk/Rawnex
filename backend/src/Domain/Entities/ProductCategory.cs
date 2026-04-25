using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class ProductCategory : BaseAuditableEntity, IAggregateRoot
{
    public string Name { get; set; } = default!;
    public string? NameFa { get; set; }
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ProductCategory? ParentCategory { get; set; }
    public ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
