using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using myYardSale.Application.Services;
using myYardSale.Domain.Entities;
using myYardSale.Web.Models;

namespace myYardSale.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ListingService _listingService;

    public AdminController(ListingService listingService)
    {
        _listingService = listingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var listings = await _listingService.GetAllAsync(cancellationToken);

        var viewModel = new AdminListingIndexViewModel
        {
            Listings = listings.Select(x => new AdminListingItemViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Status = x.Status.ToString(),
                Price = x.Price,
                Category = x.Category?.Name ?? "Uncategorized",
                CreatedAt = x.CreatedAt
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var listing = await _listingService.GetByIdAsync(id, cancellationToken);
        if (listing is null)
        {
            return NotFound();
        }

        var viewModel = new ListingFormViewModel
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            Price = listing.Price,
            Status = listing.Status,
            CategoryId = listing.CategoryId,
            Categories = (await _listingService.GetCategoriesAsync(cancellationToken))
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ListingFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = (await _listingService.GetCategoriesAsync(cancellationToken))
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList();
            return View(model);
        }

        var listing = new Listing
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description,
            Price = model.Price,
            Status = model.Status,
            CategoryId = model.CategoryId
        };

        var updated = await _listingService.UpdateAsync(listing, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _listingService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
