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

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Listing listing, CancellationToken cancellationToken)
    {
        _context.Listings.Add(listing);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
