namespace myYardSale.Domain.Entities;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public ICollection<Household> Households { get; set; } = new List<Household>();
}
