using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class Wishlist : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = "Default";

    // Navigation
    public ApplicationUser User { get; set; } = default!;
    public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
}
