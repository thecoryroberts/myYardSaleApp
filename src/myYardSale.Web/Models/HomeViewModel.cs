using Microsoft.AspNetCore.Mvc.Rendering;
using myYardSale.Domain.Entities;

namespace myYardSale.Web.Models;

public sealed class HomeViewModel
{
    public string SearchTerm { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string SortBy { get; set; } = "newest";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 9;
    public int TotalResults { get; set; }
    public IReadOnlyList<SelectListItem> Categories { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<ListingSummaryViewModel> Listings { get; set; } = Array.Empty<ListingSummaryViewModel>();
}

public sealed class ListingSummaryViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
}

public sealed class ListingDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public IReadOnlyList<ListingImageViewModel> Images { get; set; } = Array.Empty<ListingImageViewModel>();
}

public sealed class ListingImageViewModel
{
    public int Id { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string? AltText { get; set; }
}

public sealed class ListingFormViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ListingStatus Status { get; set; } = ListingStatus.Active;
    public int? CategoryId { get; set; }
    public IReadOnlyList<SelectListItem> Categories { get; set; } = Array.Empty<SelectListItem>();
    public IFormFile? ImageFile { get; set; }
    public IReadOnlyList<ListingImageViewModel> ExistingImages { get; set; } = Array.Empty<ListingImageViewModel>();
}