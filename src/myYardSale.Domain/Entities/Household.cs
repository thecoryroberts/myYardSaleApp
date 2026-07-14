namespace myYardSale.Domain.Entities;

public class Household
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public int EventId { get; set; }
    public Event? Event { get; set; }
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
