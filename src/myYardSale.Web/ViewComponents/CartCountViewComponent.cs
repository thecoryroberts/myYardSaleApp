using Microsoft.AspNetCore.Mvc;
using myYardSale.Application.Services;
using System.Security.Claims;

namespace myYardSale.Web.ViewComponents;

public sealed class CartCountViewComponent : ViewComponent
{
    private readonly ListingService _listingService;

    public CartCountViewComponent(ListingService listingService)
    {
        _listingService = listingService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (User.Identity is not { IsAuthenticated: true })
        {
            return Content(string.Empty);
        }

        var claimsUser = User as System.Security.Claims.ClaimsPrincipal;
        var userIdClaim = claimsUser?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Content(string.Empty);
        }

        var items = await _listingService.GetCartItemsAsync(userId, HttpContext.RequestAborted);
        var count = items.Sum(x => x.Quantity);

        return View(count);
    }
}