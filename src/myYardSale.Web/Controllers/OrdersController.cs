using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myYardSale.Application.Services;
using myYardSale.Web.Models;

namespace myYardSale.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly ListingService _listingService;

    public OrdersController(ListingService listingService)
    {
        _listingService = listingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var orders = await _listingService.GetOrdersAsync(userId, cancellationToken);

        var viewModel = new OrderIndexViewModel
        {
            Orders = orders.Select(x => new OrderSummaryViewModel
            {
                Id = x.Id,
                PlacedAt = x.PlacedAt,
                Status = x.Status.ToString(),
                TotalAmount = x.TotalAmount,
                ItemCount = x.Items.Count
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var order = await _listingService.GetOrderByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var isAdmin = User.IsInRole("Admin");

        // Only allow the order owner or admin to view
        if (order.UserId != userId && !isAdmin)
        {
            return Forbid();
        }

        var viewModel = new OrderDetailsViewModel
        {
            Id = order.Id,
            PlacedAt = order.PlacedAt,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            Notes = order.Notes,
            CustomerName = order.User?.FullName ?? order.User?.UserName,
            Items = order.Items.Select(x => new OrderItemViewModel
            {
                ListingId = x.ListingId,
                Title = x.Listing?.Title ?? "Unknown",
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList()
        };

        return View(viewModel);
    }
}