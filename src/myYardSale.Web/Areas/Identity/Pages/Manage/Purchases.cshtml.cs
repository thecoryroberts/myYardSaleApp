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

namespace myYardSale.Web.Areas.Identity.Pages.Manage;

[Authorize]
public class PurchasesModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ListingService _listingService;

    public PurchasesModel(UserManager<ApplicationUser> userManager, ListingService listingService)
    {
        _userManager = userManager;
        _listingService = listingService;
    }

    public List<OrderSummaryViewModel> Orders { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound();
        }

        var orders = await _listingService.GetOrdersAsync(user.Id, default);
        Orders = orders.Select(x => new OrderSummaryViewModel
        {
            Id = x.Id,
            PlacedAt = x.PlacedAt,
            Status = x.Status.ToString(),
            TotalAmount = x.TotalAmount,
            ItemCount = x.Items.Count
        }).ToList();

        return Page();
    }

    public class OrderSummaryViewModel
    {
        public int Id { get; set; }
        public DateTimeOffset PlacedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}