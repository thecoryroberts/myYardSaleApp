using Microsoft.AspNetCore.Mvc;
using myYardSale.Application.Services;
using myYardSale.Web.Models;

namespace myYardSale.Web.Controllers;

public class HomeController : Controller
{
    private readonly ListingService _listingService;

    public HomeController(ListingService listingService)
    {
        _listingService = listingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, CancellationToken cancellationToken)
    {
        var listings = await _listingService.SearchAsync(searchTerm, cancellationToken);

        var viewModel = new HomeViewModel
        {
            SearchTerm = searchTerm ?? string.Empty,
            Listings = listings.Select(x => new ListingSummaryViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Price = x.Price,
                Category = x.Category?.Name ?? "Uncategorized"
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
