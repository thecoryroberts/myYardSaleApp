namespace myYardSale.Web.Models;

public sealed class AdminListingIndexViewModel
{
    public IReadOnlyList<AdminListingItemViewModel> Listings { get; set; } = Array.Empty<AdminListingItemViewModel>();
}

public sealed class AdminListingItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
