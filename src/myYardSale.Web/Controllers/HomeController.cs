using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using myYardSale.Application.Services;
using myYardSale.Domain.Entities;
using myYardSale.Web.Models;
using myYardSale.Web.Services;

namespace myYardSale.Web.Controllers;

public class HomeController : Controller
{
    private readonly ListingService _listingService;
    private readonly ListingImageService _imageService;

    public HomeController(ListingService listingService, ListingImageService imageService)
    {
        _listingService = listingService;
        _imageService = imageService;
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
                Category = x.Category?.Name ?? "Uncategorized",
                ThumbnailUrl = x.Images.FirstOrDefault()?.StoragePath
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var listing = await _listingService.GetByIdAsync(id, cancellationToken);
        if (listing is null)
        {
            return NotFound();
        }

        return View(new ListingDetailsViewModel
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            Price = listing.Price,
            Category = listing.Category?.Name ?? "Uncategorized",
            Status = listing.Status.ToString(),
            Images = listing.Images.Select(img => new ListingImageViewModel
            {
                Id = img.Id,
                StoragePath = img.StoragePath,
                AltText = img.AltText ?? img.FileName
            }).ToList()
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var viewModel = new ListingFormViewModel
        {
            Categories = (await _listingService.GetCategoriesAsync(cancellationToken))
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ListingFormViewModel model, CancellationToken cancellationToken)
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
            Title = model.Title,
            Description = model.Description,
            Price = model.Price,
            Status = model.Status,
            CategoryId = model.CategoryId
        };

        var created = await _listingService.CreateAsync(listing, cancellationToken);

        if (model.ImageFile is not null && model.ImageFile.Length > 0)
        {
            await _imageService.UploadImageAsync(created.Id, model.ImageFile, cancellationToken);
        }

        return RedirectToAction(nameof(Index));
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

        if (model.ImageFile is not null && model.ImageFile.Length > 0)
        {
            await _imageService.UploadImageAsync(updated.Id, model.ImageFile, cancellationToken);
        }

        return RedirectToAction(nameof(Details), new { id = updated.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _listingService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int imageId, int listingId, CancellationToken cancellationToken)
    {
        await _imageService.DeleteImageAsync(imageId, cancellationToken);
        return RedirectToAction(nameof(Edit), new { id = listingId });
    }

    [HttpGet]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}
