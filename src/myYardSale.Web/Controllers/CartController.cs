using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myYardSale.Application.Services;
using myYardSale.Web.Models;

namespace myYardSale.Web.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ListingService _listingService;

    public CartController(ListingService listingService)
    {
        _listingService = listingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var cartItems = await _listingService.GetCartItemsAsync(userId, cancellationToken);

        var viewModel = new CartIndexViewModel
        {
            Items = cartItems.Select(x => new CartItemViewModel
            {
                Id = x.Id,
                ListingId = x.ListingId,
                Title = x.Listing?.Title ?? "Unknown",
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity,
                ThumbnailUrl = x.Listing?.Images.FirstOrDefault()?.StoragePath
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int listingId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _listingService.AddToCartAsync(listingId, userId, cancellationToken);
        TempData["Success"] = "Item added to cart.";
        return RedirectToAction("Details", "Home", new { id = listingId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId, CancellationToken cancellationToken)
    {
        await _listingService.RemoveFromCartAsync(cartItemId, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var cartItems = await _listingService.GetCartItemsAsync(userId, cancellationToken);

        if (!cartItems.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction(nameof(Index));
        }

        var viewModel = new CheckoutViewModel
        {
            Items = cartItems.Select(x => new CartItemViewModel
            {
                Id = x.Id,
                ListingId = x.ListingId,
                Title = x.Listing?.Title ?? "Unknown",
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity,
                ThumbnailUrl = x.Listing?.Images.FirstOrDefault()?.StoragePath
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        try
        {
            var order = await _listingService.CheckoutAsync(userId, model.Notes, cancellationToken);
            TempData["Success"] = $"Order #{order.Id} placed successfully!";
            return RedirectToAction("Details", "Orders", new { id = order.Id });
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}