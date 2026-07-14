using myYardSale.Domain.Entities;

namespace myYardSale.Application.Abstractions;

public interface IListingRepository
{
    Task<IReadOnlyList<Listing>> GetActiveListingsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken);
}
