using myYardSale.Application.Abstractions;
using myYardSale.Domain.Entities;

namespace myYardSale.Application.Services;

public sealed class ListingService
{
    private readonly IListingRepository _listingRepository;

    public ListingService(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<IReadOnlyList<Listing>> SearchAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var listings = await _listingRepository.GetActiveListingsAsync(cancellationToken);
        var normalizedTerm = searchTerm?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedTerm))
        {
            return listings;
        }

        return listings
            .Where(x =>
                x.Title.Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase) ||
                x.Description.Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase) ||
                x.Category?.Name.Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase) == true)
            .ToList();
    }

    public Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => _listingRepository.GetByIdAsync(id, cancellationToken);

    public async Task<Listing> CreateAsync(Listing listing, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(listing);

        if (string.IsNullOrWhiteSpace(listing.Title))
        {
            throw new ArgumentException("Title is required.", nameof(listing));
        }

        if (listing.Price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(listing), "Price cannot be negative.");
        }

        listing.Status = listing.Status == default ? ListingStatus.Active : listing.Status;
        listing.CreatedAt = DateTimeOffset.UtcNow;

        return await _listingRepository.AddAsync(listing, cancellationToken);
    }

    public Task<Listing?> UpdateAsync(Listing listing, CancellationToken cancellationToken)
        => _listingRepository.UpdateAsync(listing, cancellationToken);

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        => _listingRepository.DeleteAsync(id, cancellationToken);

    public Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
        => _listingRepository.GetCategoriesAsync(cancellationToken);
}
