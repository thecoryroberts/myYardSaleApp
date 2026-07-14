using myYardSale.Domain.Entities;

namespace myYardSale.Web.Models;

public sealed class OrderIndexViewModel
{
    public IReadOnlyList<OrderSummaryViewModel> Orders { get; set; } = Array.Empty<OrderSummaryViewModel>();
}

public sealed class OrderSummaryViewModel
{
    public int Id { get; set; }
    public DateTimeOffset PlacedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}

public sealed class OrderDetailsViewModel
{
    public int Id { get; set; }
    public DateTimeOffset PlacedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string? CustomerName { get; set; }
    public IReadOnlyList<OrderItemViewModel> Items { get; set; } = Array.Empty<OrderItemViewModel>();
}

public sealed class OrderItemViewModel
{
    public int ListingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
}