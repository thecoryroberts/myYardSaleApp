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
            .ToListAsync(cancellationToken);

        return listings
            .Where(x => x.Status == ListingStatus.Active)
            .OrderByDescending(x => x.CreatedAt.DateTime)
            .ToList();
    }

    public async Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Listings
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderByDescending(x => x.CreatedAt.DateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Listings
            .AsNoTracking()
            .Include(x => x.Category)
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

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var listing = await _context.Listings.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (listing is null)
        {
            return false;
        }

        _context.Listings.Remove(listing);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
