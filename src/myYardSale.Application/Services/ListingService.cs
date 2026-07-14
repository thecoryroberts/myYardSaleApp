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
        ArgumentNullException.ThrowIfNull(searchTerm);

        var listings = await _listingRepository.GetActiveListingsAsync(cancellationToken);
        var normalizedTerm = searchTerm.Trim();

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
}
