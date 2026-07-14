using myYardSale.Application.Abstractions;
using myYardSale.Domain.Entities;

namespace myYardSale.Infrastructure.Persistence;

public sealed class InMemoryListingRepository : IListingRepository
{
    private readonly IReadOnlyList<Listing> _listings =
    [
        new Listing
        {
            Id = 1,
            Title = "Vintage Bicycle",
            Description = "A reliable bike for weekend rides and daily errands.",
            Price = 75m,
            Status = ListingStatus.Active,
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Sports" }
        },
        new Listing
        {
            Id = 2,
            Title = "Coffee Table",
            Description = "Solid oak table with minor wear and a great finish.",
            Price = 120m,
            Status = ListingStatus.Active,
            CategoryId = 2,
            Category = new Category { Id = 2, Name = "Furniture" }
        },
        new Listing
        {
            Id = 3,
            Title = "Lawn Mower",
            Description = "Gas-powered mower ready for the season.",
            Price = 180m,
            Status = ListingStatus.Active,
            CategoryId = 3,
            Category = new Category { Id = 3, Name = "Tools" }
        }
    ];

    public Task<IReadOnlyList<Listing>> GetActiveListingsAsync(CancellationToken cancellationToken)
        => Task.FromResult(_listings.Where(x => x.Status == ListingStatus.Active).ToList() as IReadOnlyList<Listing>);

    public Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken)
        => Task.FromResult(_listings.ToList() as IReadOnlyList<Listing>);

    public Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => Task.FromResult(_listings.FirstOrDefault(x => x.Id == id));

    public Task<Listing> AddAsync(Listing listing, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(listing);
        return Task.FromResult(listing);
    }

    public Task<Listing?> UpdateAsync(Listing listing, CancellationToken cancellationToken)
        => Task.FromResult<Listing?>(listing);

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        => Task.FromResult(true);

    public Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Category>>(_listings.Select(x => x.Category!).Where(x => x is not null).DistinctBy(x => x.Id).ToList());
}
