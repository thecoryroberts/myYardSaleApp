using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using myYardSale.Application.Services;
using myYardSale.Domain.Entities;

namespace myYardSale.Web.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ListingService _listingService;

    public DashboardModel(UserManager<ApplicationUser> userManager, ListingService listingService)
    {
        _userManager = userManager;
        _listingService = listingService;
    }

    public int ActiveListings { get; set; }
    public int TotalPurchases { get; set; }
    public int PendingOrders { get; set; }
    public int CartCount { get; set; }
    public List<RecentListingItem> RecentListings { get; set; } = new();
    public List<RecentOrderItem> RecentOrders { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var userId = user.Id;

        // Active listings
        var myListings = await _listingService.GetByUserAsync(userId, default);
        ActiveListings = myListings.Count(l => l.Status == ListingStatus.Active);
        RecentListings = myListings
            .OrderByDescending(l => l.CreatedAt)
            .Take(5)
            .Select(l => new RecentListingItem
            {
                Id = l.Id,
                Title = l.Title,
                Price = l.Price
            })
            .ToList();

        // Orders / purchases
        var orders = await _listingService.GetOrdersAsync(userId, default);
        TotalPurchases = orders.Count;
        PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending);
        RecentOrders = orders
            .OrderByDescending(o => o.PlacedAt)
            .Take(5)
            .Select(o => new RecentOrderItem
            {
                Id = o.Id,
                Status = o.Status.ToString(),
                StatusClass = o.Status switch
                {
                    OrderStatus.Pending => "bg-warning",
                    OrderStatus.Confirmed => "bg-info",
                    OrderStatus.Shipped => "bg-primary",
                    OrderStatus.Delivered => "bg-success",
                    OrderStatus.Cancelled => "bg-danger",
                    _ => "bg-secondary"
                }
            })
            .ToList();

        // Cart count
        var cartItems = await _listingService.GetCartItemsAsync(userId, default);
        CartCount = cartItems.Count;

        return Page();
    }

    public class RecentListingItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class RecentOrderItem
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusClass { get; set; } = string.Empty;
    }
}