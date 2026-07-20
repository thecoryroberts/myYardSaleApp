namespace myYardSale.Domain.Entities;

public class CartItem : BaseEntity
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public Listing? Listing { get; set; }
    public int UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public DateTimeOffset AddedAt { get; set; } = DateTimeOffset.UtcNow;
}