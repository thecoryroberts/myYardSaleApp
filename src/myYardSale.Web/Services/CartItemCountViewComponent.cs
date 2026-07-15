using Microsoft.AspNetCore.Mvc;
using myYardSale.Application.Services;

namespace myYardSale.Web.Services;

public class CartItemCountViewComponent : ViewComponent
{
    private readonly ListingService _listingService;

    public CartItemCountViewComponent(ListingService listingService)
    {
        _listingService = listingService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = int.Parse(HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var cartItems = await _listingService.GetCartItemsAsync(userId, CancellationToken.None);
        var count = cartItems.Sum(x => x.Quantity);
        return View(count);
    }
}