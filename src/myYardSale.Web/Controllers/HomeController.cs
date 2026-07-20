using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchTerm,
        int? categoryId,
        string? sortBy,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        const int pageSize = 9;
        var effectiveSort = string.IsNullOrWhiteSpace(sortBy) ? "newest" : sortBy;
        var effectivePage = page < 1 ? 1 : page;

        var categories = await _listingService.GetCategoriesAsync(cancellationToken);
        var allMatches = await _listingService.SearchAsync(searchTerm, categoryId, effectiveSort, cancellationToken);

        var totalResults = allMatches.Count;
        var paged = allMatches
            .Skip((effectivePage - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ListingSummaryViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Price = x.Price,
                Category = x.Category?.Name ?? "Uncategorized",
                ThumbnailUrl = x.Images.FirstOrDefault()?.StoragePath
            })
            .ToList();

        var viewModel = new HomeViewModel
        {
            SearchTerm = searchTerm ?? string.Empty,
            CategoryId = categoryId,
            SortBy = effectiveSort,
            Page = effectivePage,
            PageSize = pageSize,
            TotalResults = totalResults,
            Categories = categories
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList(),
            Listings = paged
        };

        return View(viewModel);
    }

    [HttpGet]
    [Authorize(Policy = "CanManageListings")]
    public async Task<IActionResult> MyListings(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var listings = await _listingService.GetByUserAsync(userId, cancellationToken);

        var viewModel = new HomeViewModel
        {
            SearchTerm = string.Empty,
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
    [Authorize(Policy = "CanManageListings")]
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
    [Authorize(Policy = "CanManageListings")]
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
            CategoryId = model.CategoryId,
            UserId = GetCurrentUserId()
        };

        var created = await _listingService.CreateAsync(listing, cancellationToken);

        if (model.ImageFiles is { Count: > 0 })
        {
            foreach (var file in model.ImageFiles)
            {
                if (file.Length > 0)
                {
                    await _imageService.UploadImageAsync(created.Id, file, cancellationToken);
                }
            }
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "CanManageListings")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var listing = await _listingService.GetByIdAsync(id, cancellationToken);
        if (listing is null)
        {
            return NotFound();
        }

        // Only allow the owner or admin to edit
        var currentUserId = GetCurrentUserId();
        if (!User.IsInRole("Admin") && listing.UserId != currentUserId)
        {
            return Forbid();
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
    [Authorize(Policy = "CanManageListings")]
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
            CategoryId = model.CategoryId,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var updated = await _listingService.UpdateAsync(listing, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

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

        return RedirectToAction(nameof(Details), new { id = updated.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "CanManageListings")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var listing = await _listingService.GetByIdAsync(id, cancellationToken);
        if (listing is null)
        {
            return NotFound();
        }

        // Only allow the owner or admin to delete
        var currentUserId = GetCurrentUserId();
        if (!User.IsInRole("Admin") && listing.UserId != currentUserId)
        {
            return Forbid();
        }

        await _listingService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "CanManageListings")]
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
