using myYardSale.Application.Abstractions;
using myYardSale.Domain.Entities;

namespace myYardSale.Infrastructure.Persistence;

public sealed class InMemoryListingRepository : IListingRepository
{
    private readonly List<Listing> _listings =
    [
        new Listing
        {
            Id = 1,
            Title = "Vintage Bicycle",
            Description = "A reliable bike for weekend rides and daily errands.",
            Price = 75m,
            Status = ListingStatus.Active,
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Sports" },
            UserId = 1
        },
        new Listing
        {
            Id = 2,
            Title = "Coffee Table",
            Description = "Solid oak table with minor wear and a great finish.",
            Price = 120m,
            Status = ListingStatus.Active,
            CategoryId = 2,
            Category = new Category { Id = 2, Name = "Furniture" },
            UserId = 1
        },
        new Listing
        {
            Id = 3,
            Title = "Lawn Mower",
            Description = "Gas-powered mower ready for the season.",
            Price = 180m,
            Status = ListingStatus.Active,
            CategoryId = 3,
            Category = new Category { Id = 3, Name = "Tools" },
            UserId = 2
        }
    ];

    public Task<IReadOnlyList<Listing>> GetActiveListingsAsync(CancellationToken cancellationToken)
        => Task.FromResult(_listings.Where(x => x.Status == ListingStatus.Active).ToList() as IReadOnlyList<Listing>);

    public Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken)
        => Task.FromResult(_listings.ToList() as IReadOnlyList<Listing>);

    public Task<IReadOnlyList<Listing>> GetListingsByUserAsync(int userId, CancellationToken cancellationToken)
        => Task.FromResult(_listings.Where(x => x.UserId == userId).ToList() as IReadOnlyList<Listing>);

    public Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => Task.FromResult(_listings.FirstOrDefault(x => x.Id == id));

    public Task<Listing> AddAsync(Listing listing, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(listing);
        if (listing.Id == 0)
        {
            listing.Id = _listings.Count == 0 ? 1 : _listings.Max(x => x.Id) + 1;
        }
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
