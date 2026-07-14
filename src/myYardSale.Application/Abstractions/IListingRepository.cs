using myYardSale.Domain.Entities;

namespace myYardSale.Application.Abstractions;

public interface IListingRepository
{
    Task<IReadOnlyList<Listing>> GetActiveListingsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken);
    Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Listing> AddAsync(Listing listing, CancellationToken cancellationToken);
    Task<Listing?> UpdateAsync(Listing listing, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<ListingImage> AddImageAsync(int listingId, ListingImage image, CancellationToken cancellationToken);
    Task<bool> DeleteImageAsync(int imageId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ListingImage>> GetImagesAsync(int listingId, CancellationToken cancellationToken);
}
