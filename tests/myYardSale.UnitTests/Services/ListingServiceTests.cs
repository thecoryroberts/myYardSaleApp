using myYardSale.Application.Abstractions;
using myYardSale.Application.Services;
using myYardSale.Domain.Entities;

namespace myYardSale.UnitTests.Services;

public class ListingServiceTests
{
    [Fact]
    public async Task SearchAsync_ReturnsMatchingListings()
    {
        var repository = new FakeListingRepository();
        var service = new ListingService(repository);

        var result = await service.SearchAsync("bicycle", CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Vintage Bicycle", result[0].Title);
    }

    private sealed class FakeListingRepository : IListingRepository
    {
        private readonly List<Listing> _listings =
        [
            new Listing
            {
                Id = 1,
                Title = "Vintage Bicycle",
                Description = "Perfect for weekend rides",
                Price = 75m,
                Status = ListingStatus.Active,
                Category = new Category { Id = 1, Name = "Sports" }
            },
            new Listing
            {
                Id = 2,
                Title = "Coffee Table",
                Description = "Solid oak table",
                Price = 120m,
                Status = ListingStatus.Active,
                Category = new Category { Id = 2, Name = "Furniture" }
            }
        ];

        public Task<IReadOnlyList<Listing>> GetActiveListingsAsync(CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Listing>>(_listings.Where(x => x.Status == ListingStatus.Active).ToList());

        public Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Category>>(_listings.Select(x => x.Category!).Where(x => x is not null).DistinctBy(x => x.Id).ToList());
    }
}
