namespace myYardSale.Domain.Entities;

public class Listing
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ListingStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Category? Category { get; set; }
    public int? CategoryId { get; set; }
    public Household? Household { get; set; }
    public int? HouseholdId { get; set; }
    public int? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();
}

public enum ListingStatus
{
    Draft = 0,
    Active = 1,
    Sold = 2,
    Removed = 3
}
