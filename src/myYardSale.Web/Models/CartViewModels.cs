namespace myYardSale.Web.Models;

public sealed class CartIndexViewModel
{
    public IReadOnlyList<CartItemViewModel> Items { get; set; } = Array.Empty<CartItemViewModel>();
    public decimal Total => Items.Sum(x => x.Subtotal);
    public int ItemCount => Items.Count;
}

public sealed class CartItemViewModel
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
    public string? ThumbnailUrl { get; set; }
}

public sealed class CheckoutViewModel
{
    public IReadOnlyList<CartItemViewModel> Items { get; set; } = Array.Empty<CartItemViewModel>();
    public decimal Total => Items.Sum(x => x.Subtotal);
    public string? Notes { get; set; }
}