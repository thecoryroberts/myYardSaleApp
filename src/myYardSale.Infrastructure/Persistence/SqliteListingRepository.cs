using Microsoft.EntityFrameworkCore;
using myYardSale.Application.Abstractions;
using myYardSale.Domain.Entities;

namespace myYardSale.Infrastructure.Persistence;

public sealed class SqliteListingRepository : IListingRepository
{
    private readonly MyYardSaleDbContext _context;

    public SqliteListingRepository(MyYardSaleDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Listing>> GetActiveListingsAsync(CancellationToken cancellationToken)
    {
        var listings = await _context.Listings
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Images)
            .ToListAsync(cancellationToken);

        return listings
            .Where(x => x.Status == ListingStatus.Active)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken)
    {
        var listings = await _context.Listings
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Images)
            .ToListAsync(cancellationToken);

        return listings.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public async Task<IReadOnlyList<Listing>> GetListingsByUserAsync(int userId, CancellationToken cancellationToken)
    {
        var listings = await _context.Listings
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Images)
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        return listings.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public async Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Listings
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Listing> AddAsync(Listing listing, CancellationToken cancellationToken)
    {
        _context.Listings.Add(listing);
        await _context.SaveChangesAsync(cancellationToken);
        return listing;
    }

    public async Task<Listing?> UpdateAsync(Listing listing, CancellationToken cancellationToken)
    {
        var existing = await _context.Listings.FirstOrDefaultAsync(x => x.Id == listing.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Title = listing.Title;
        existing.Description = listing.Description;
        existing.Price = listing.Price;
        existing.Status = listing.Status;
        existing.CategoryId = listing.CategoryId;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<ListingImage> AddImageAsync(int listingId, ListingImage image, CancellationToken cancellationToken)
    {
        image.ListingId = listingId;
        _context.ListingImages.Add(image);
        await _context.SaveChangesAsync(cancellationToken);
        return image;
    }

    public async Task<bool> DeleteImageAsync(int imageId, CancellationToken cancellationToken)
    {
        var image = await _context.ListingImages.FirstOrDefaultAsync(x => x.Id == imageId, cancellationToken);
        if (image is null)
        {
            return false;
        }

        _context.ListingImages.Remove(image);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<ListingImage>> GetImagesAsync(int listingId, CancellationToken cancellationToken)
    {
        return await _context.ListingImages
            .AsNoTracking()
            .Where(x => x.ListingId == listingId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<ListingImage?> GetImageByIdAsync(int imageId, CancellationToken cancellationToken)
    {
        return await _context.ListingImages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == imageId, cancellationToken);
    }

    // Cart
    public async Task<CartItem> AddToCartAsync(CartItem item, CancellationToken cancellationToken)
    {
        _context.CartItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<bool> RemoveFromCartAsync(int cartItemId, CancellationToken cancellationToken)
    {
        var item = await _context.CartItems.FirstOrDefaultAsync(x => x.Id == cartItemId, cancellationToken);
        if (item is null) return false;
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<CartItem>> GetCartItemsAsync(int userId, CancellationToken cancellationToken)
    {
        var items = await _context.CartItems
            .AsNoTracking()
            .Include(x => x.Listing!)
                .ThenInclude(x => x.Images)
            .Include(x => x.Listing!.Category)
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        return items.OrderByDescending(x => x.AddedAt).ToList();
    }

    public async Task<bool> ClearCartAsync(int userId, CancellationToken cancellationToken)
    {
        var items = await _context.CartItems.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId, CancellationToken cancellationToken)
        => await _context.CartItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == cartItemId, cancellationToken);

    public async Task<CartItem?> GetCartItemByListingAndUserAsync(int listingId, int userId, CancellationToken cancellationToken)
        => await _context.CartItems
            .FirstOrDefaultAsync(x => x.ListingId == listingId && x.UserId == userId, cancellationToken);

    // Orders
    public async Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<IReadOnlyList<Order>> GetOrdersAsync(int userId, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .ThenInclude(x => x.Listing)
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        return orders.OrderByDescending(x => x.PlacedAt).ToList();
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .ThenInclude(x => x.Listing)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetAllOrdersAsync(CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .ThenInclude(x => x.Listing)
            .Include(x => x.User)
            .ToListAsync(cancellationToken);

        return orders.OrderByDescending(x => x.PlacedAt).ToList();
    }

    public async Task<Order?> UpdateOrderStatusAsync(int orderId, OrderStatus status, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
        if (order is null) return null;
        order.Status = status;
        await _context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var listing = await _context.Listings
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (listing is null)
        {
            return false;
        }

        _context.Listings.Remove(listing);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
