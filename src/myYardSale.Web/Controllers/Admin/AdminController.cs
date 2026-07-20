using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using myYardSale.Application.Services;
using myYardSale.Domain.Entities;
using myYardSale.Web.Models;
using myYardSale.Web.Services;

namespace myYardSale.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ListingService _listingService;
    private readonly ListingImageService _imageService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        ListingService listingService,
        ListingImageService imageService,
        ILogger<AdminController> logger)
    {
        _listingService = listingService;
        _imageService = imageService;
        _logger = logger;
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
                .ToList(),
            ExistingImages = listing.Images.Select(img => new ListingImageViewModel
            {
                Id = img.Id,
                StoragePath = img.StoragePath,
                AltText = img.AltText ?? img.FileName
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ListingFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            model.Categories = (await _listingService.GetCategoriesAsync(cancellationToken))
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList();
            return View(model);
        }

        var existing = await _listingService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        var listing = new Listing
        {
            Id = id,
            Title = model.Title,
            Description = model.Description,
            Price = model.Price,
            Status = model.Status,
            CategoryId = model.CategoryId,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var updated = await _listingService.UpdateAsync(listing, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        _logger.LogInformation("Admin updated listing {ListingId}", id);

        if (model.ImageFiles is { Count: > 0 })
        {
            foreach (var file in model.ImageFiles)
            {
                if (file.Length > 0)
                {
                    await _imageService.UploadImageAsync(updated.Id, file, cancellationToken);
                }
            }
        }

        TempData["Success"] = "Listing updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await _listingService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        await _listingService.DeleteAsync(id, cancellationToken);
        _logger.LogInformation("Admin deleted listing {ListingId}", id);
        TempData["Success"] = "Listing deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int imageId, int listingId, CancellationToken cancellationToken)
    {
        var listing = await _listingService.GetByIdAsync(listingId, cancellationToken);
        if (listing is null)
        {
            return NotFound();
        }

        await _imageService.DeleteImageAsync(imageId, cancellationToken);
        _logger.LogInformation("Admin deleted image {ImageId} from listing {ListingId}", imageId, listingId);
        TempData["Success"] = "Image deleted.";
        return RedirectToAction(nameof(Edit), new { id = listingId });
    }
}