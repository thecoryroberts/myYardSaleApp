using Microsoft.AspNetCore.Identity;

namespace myYardSale.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string? FullName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? PickupNotes { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactNotes { get; set; }
}
