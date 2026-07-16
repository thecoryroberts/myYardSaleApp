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

    public Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken)
        => _listingRepository.GetAllAsync(cancellationToken);

    public Task<IReadOnlyList<Listing>> GetByUserAsync(int userId, CancellationToken cancellationToken)
        => _listingRepository.GetListingsByUserAsync(userId, cancellationToken);

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

    public Task<ListingImage> AddImageAsync(int listingId, ListingImage image, CancellationToken cancellationToken)
        => _listingRepository.AddImageAsync(listingId, image, cancellationToken);

    public Task<bool> DeleteImageAsync(int imageId, CancellationToken cancellationToken)
        => _listingRepository.DeleteImageAsync(imageId, cancellationToken);

    public Task<IReadOnlyList<ListingImage>> GetImagesAsync(int listingId, CancellationToken cancellationToken)
        => _listingRepository.GetImagesAsync(listingId, cancellationToken);

    public Task<ListingImage?> GetImageByIdAsync(int imageId, CancellationToken cancellationToken)
        => _listingRepository.GetImageByIdAsync(imageId, cancellationToken);

    // Cart
    public async Task<CartItem> AddToCartAsync(int listingId, int userId, CancellationToken cancellationToken)
    {
        var existing = await _listingRepository.GetCartItemByListingAndUserAsync(listingId, userId, cancellationToken);
        if (existing is not null)
        {
            existing.Quantity++;
            return existing;
        }

        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null)
        {
            throw new InvalidOperationException("Listing not found.");
        }

        var item = new CartItem
        {
            ListingId = listingId,
            UserId = userId,
            Quantity = 1,
            UnitPrice = listing.Price,
            AddedAt = DateTimeOffset.UtcNow
        };

        return await _listingRepository.AddToCartAsync(item, cancellationToken);
    }

    public Task<bool> RemoveFromCartAsync(int cartItemId, CancellationToken cancellationToken)
        => _listingRepository.RemoveFromCartAsync(cartItemId, cancellationToken);

    public Task<IReadOnlyList<CartItem>> GetCartItemsAsync(int userId, CancellationToken cancellationToken)
        => _listingRepository.GetCartItemsAsync(userId, cancellationToken);

    public Task<bool> ClearCartAsync(int userId, CancellationToken cancellationToken)
        => _listingRepository.ClearCartAsync(userId, cancellationToken);

    // Orders
    public async Task<Order> CheckoutAsync(int userId, string? notes, CancellationToken cancellationToken)
    {
        var cartItems = await _listingRepository.GetCartItemsAsync(userId, cancellationToken);
        if (!cartItems.Any())
        {
            throw new InvalidOperationException("Cart is empty.");
        }

        var order = new Order
        {
            UserId = userId,
            PlacedAt = DateTimeOffset.UtcNow,
            Status = OrderStatus.Pending,
            Notes = notes,
            TotalAmount = cartItems.Sum(x => x.UnitPrice * x.Quantity),
            Items = cartItems.Select(x => new OrderItem
            {
                ListingId = x.ListingId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList()
        };

        var created = await _listingRepository.CreateOrderAsync(order, cancellationToken);
        await _listingRepository.ClearCartAsync(userId, cancellationToken);
        return created;
    }

    public Task<IReadOnlyList<Order>> GetOrdersAsync(int userId, CancellationToken cancellationToken)
        => _listingRepository.GetOrdersAsync(userId, cancellationToken);

    public Task<Order?> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken)
        => _listingRepository.GetOrderByIdAsync(orderId, cancellationToken);

    public Task<IReadOnlyList<Order>> GetAllOrdersAsync(CancellationToken cancellationToken)
        => _listingRepository.GetAllOrdersAsync(cancellationToken);

    public Task<Order?> UpdateOrderStatusAsync(int orderId, OrderStatus status, CancellationToken cancellationToken)
        => _listingRepository.UpdateOrderStatusAsync(orderId, status, cancellationToken);
}