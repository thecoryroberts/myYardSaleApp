using Microsoft.Extensions.Caching.Memory;
using myYardSale.Application.Abstractions;
using myYardSale.Application.Services;
using myYardSale.Domain.Entities;

namespace myYardSale.UnitTests.Services;

public class ListingServiceTests
{
    private static ListingService CreateService(IListingRepository repository)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        return new ListingService(repository, cache);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingListings()
    {
        var repository = new FakeListingRepository();
        var service = CreateService(repository);

        var result = await service.SearchAsync("bicycle", null, null, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Vintage Bicycle", result[0].Title);
    }

    [Fact]
    public async Task SearchAsync_WithNullTerm_ReturnsAllActiveListings()
    {
        var repository = new FakeListingRepository();
        var service = CreateService(repository);

        var result = await service.SearchAsync(null, null, null, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Title == "Vintage Bicycle");
        Assert.Contains(result, x => x.Title == "Coffee Table");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsRequestedListing()
    {
        var repository = new FakeListingRepository();
        var service = CreateService(repository);

        var result = await service.GetByIdAsync(2, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Coffee Table", result!.Title);
    }

    [Fact]
    public async Task CreateAsync_AddsListingAndReturnsIt()
    {
        var repository = new FakeListingRepository();
        var service = CreateService(repository);

        var created = await service.CreateAsync(new Listing
        {
            Title = "Desk Lamp",
            Description = "Modern desk lamp",
            Price = 35m,
            Status = ListingStatus.Active,
            Category = new Category { Id = 3, Name = "Decor" }
        }, CancellationToken.None);

        Assert.NotNull(created);
        Assert.Equal("Desk Lamp", created.Title);
        Assert.Equal(3, repository.GetCount());
    }

    [Fact]
    public async Task GetCategoriesAsync_UsesCaching()
    {
        var repository = new FakeListingRepository();
        var service = CreateService(repository);

        // First call should fetch from repository
        var firstCall = await service.GetCategoriesAsync(CancellationToken.None);
        Assert.NotEmpty(firstCall);

        // Second call should return cached result (no exception)
        var secondCall = await service.GetCategoriesAsync(CancellationToken.None);
        Assert.Equal(firstCall.Count, secondCall.Count);
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

        public int GetCount() => _listings.Count;

        public Task<IReadOnlyList<Listing>> GetActiveListingsAsync(CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Listing>>(_listings.Where(x => x.Status == ListingStatus.Active).ToList());

        public Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Listing>>(_listings.ToList());

        public Task<IReadOnlyList<Listing>> GetListingsByUserAsync(int userId, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Listing>>(_listings.Where(x => x.UserId == userId).ToList());

        public Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(_listings.FirstOrDefault(x => x.Id == id));

        public Task<Listing> AddAsync(Listing listing, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(listing);
            _listings.Add(listing);
            return Task.FromResult(listing);
        }

        public Task<Listing?> UpdateAsync(Listing listing, CancellationToken cancellationToken)
            => Task.FromResult<Listing?>(listing);

        public Task<ListingImage> AddImageAsync(int listingId, ListingImage image, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(image);
            return Task.FromResult(image);
        }

        public Task<bool> DeleteImageAsync(int imageId, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<IReadOnlyList<ListingImage>> GetImagesAsync(int listingId, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<ListingImage>>([]);

        public Task<ListingImage?> GetImageByIdAsync(int imageId, CancellationToken cancellationToken)
            => Task.FromResult<ListingImage?>(null);

        // Cart
        public Task<CartItem> AddToCartAsync(CartItem item, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(item);
            return Task.FromResult(item);
        }

        public Task<bool> RemoveFromCartAsync(int cartItemId, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<IReadOnlyList<CartItem>> GetCartItemsAsync(int userId, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<CartItem>>([]);

        public Task<bool> ClearCartAsync(int userId, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<CartItem?> GetCartItemByIdAsync(int cartItemId, CancellationToken cancellationToken)
            => Task.FromResult<CartItem?>(null);

        public Task<CartItem?> GetCartItemByListingAndUserAsync(int listingId, int userId, CancellationToken cancellationToken)
            => Task.FromResult<CartItem?>(null);

        // Orders
        public Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(order);
            return Task.FromResult(order);
        }

        public Task<IReadOnlyList<Order>> GetOrdersAsync(int userId, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Order>>([]);

        public Task<Order?> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken)
            => Task.FromResult<Order?>(null);

        public Task<IReadOnlyList<Order>> GetAllOrdersAsync(CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Order>>([]);

        public Task<Order?> UpdateOrderStatusAsync(int orderId, OrderStatus status, CancellationToken cancellationToken)
            => Task.FromResult<Order?>(null);

        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Category>>(_listings.Select(x => x.Category!).Where(x => x is not null).DistinctBy(x => x.Id).ToList());
    }
}