using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class WishlistItem : BaseEntity
{
    public Guid WishlistId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Wishlist Wishlist { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
