using myYardSale.Domain.Entities;

namespace myYardSale.Application.Abstractions;

public interface IListingRepository
{
    Task<IReadOnlyList<Listing>> GetActiveListingsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Listing>> GetListingsByUserAsync(int userId, CancellationToken cancellationToken);
    Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Listing> AddAsync(Listing listing, CancellationToken cancellationToken);
    Task<Listing?> UpdateAsync(Listing listing, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<ListingImage> AddImageAsync(int listingId, ListingImage image, CancellationToken cancellationToken);
    Task<bool> DeleteImageAsync(int imageId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ListingImage>> GetImagesAsync(int listingId, CancellationToken cancellationToken);
    Task<ListingImage?> GetImageByIdAsync(int imageId, CancellationToken cancellationToken);

    // Cart
    Task<CartItem> AddToCartAsync(CartItem item, CancellationToken cancellationToken);
    Task<bool> RemoveFromCartAsync(int cartItemId, CancellationToken cancellationToken);
    Task<IReadOnlyList<CartItem>> GetCartItemsAsync(int userId, CancellationToken cancellationToken);
    Task<bool> ClearCartAsync(int userId, CancellationToken cancellationToken);
    Task<CartItem?> GetCartItemByIdAsync(int cartItemId, CancellationToken cancellationToken);
    Task<CartItem?> GetCartItemByListingAndUserAsync(int listingId, int userId, CancellationToken cancellationToken);

    // Orders
    Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> GetOrdersAsync(int userId, CancellationToken cancellationToken);
    Task<Order?> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> GetAllOrdersAsync(CancellationToken cancellationToken);
    Task<Order?> UpdateOrderStatusAsync(int orderId, OrderStatus status, CancellationToken cancellationToken);
}
