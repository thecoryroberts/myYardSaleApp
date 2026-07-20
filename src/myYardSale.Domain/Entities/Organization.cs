namespace myYardSale.Domain.Entities;

public class Organization : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Event> Events { get; set; } = new List<Event>();
}
