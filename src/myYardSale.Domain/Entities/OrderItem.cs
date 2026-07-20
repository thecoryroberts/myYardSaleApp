namespace myYardSale.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ListingId { get; set; }
    public Listing? Listing { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}