using myYardSale.Application.Services;
using myYardSale.Domain.Entities;

namespace myYardSale.Web.Services;

public sealed class ListingImageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp"
    };

    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    private readonly ListingService _listingService;
    private readonly IWebHostEnvironment _environment;

    public ListingImageService(ListingService listingService, IWebHostEnvironment environment)
    {
        _listingService = listingService;
        _environment = environment;
    }

    public async Task<ListingImage> UploadImageAsync(int listingId, IFormFile file, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file);

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"File type '{extension}' is not supported. Allowed types: {string.Join(", ", AllowedExtensions)}");
        }

        if (file.Length == 0)
        {
            throw new InvalidOperationException("Uploaded file is empty.");
        }

        if (file.Length > MaxFileSize)
        {
            throw new InvalidOperationException($"File size exceeds the maximum allowed size of 5 MB.");
        }

        var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "listings", listingId.ToString());
        Directory.CreateDirectory(uploadsDir);

        var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsDir, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var storagePath = $"/uploads/listings/{listingId}/{uniqueFileName}";

        var image = new ListingImage
        {
            FileName = file.FileName,
            StoragePath = storagePath,
            SortOrder = 0,
            UploadedAt = DateTimeOffset.UtcNow
        };

        return await _listingService.AddImageAsync(listingId, image, cancellationToken);
    }

    public async Task<bool> DeleteImageAsync(int imageId, CancellationToken cancellationToken)
    {
        var image = await _listingService.GetImageByIdAsync(imageId, cancellationToken);
        if (image is null)
        {
            return false;
        }

        var filePath = _environment.WebRootPath + image.StoragePath;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return await _listingService.DeleteImageAsync(imageId, cancellationToken);
    }
}