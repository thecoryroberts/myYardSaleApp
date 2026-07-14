using myYardSale.Domain.Entities;

namespace myYardSale.Application.Abstractions;

public interface IListingRepository
{
    Task<IReadOnlyList<Listing>> GetActiveListingsAsync(CancellationToken cancellationToken);
    Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Listing> AddAsync(Listing listing, CancellationToken cancellationToken);
    Task<Listing?> UpdateAsync(Listing listing, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken);
}
