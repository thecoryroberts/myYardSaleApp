using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using myYardSale.Infrastructure.Persistence;
using myYardSale.Domain.Entities;

namespace myYardSale.IntegrationTests.Persistence;

public class SqliteListingRepositoryTests
{
    [Fact]
    public async Task SaveAndLoadListings_WithSqlite_Works()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<MyYardSaleDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new MyYardSaleDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new SqliteListingRepository(context);
        var listing = new Listing
        {
            Title = "Test Listing",
            Description = "A listing saved to SQLite",
            Price = 25m,
            Status = ListingStatus.Active,
            Category = new Category { Name = "Furniture" }
        };

        await repository.AddAsync(listing, CancellationToken.None);

        var results = await repository.GetActiveListingsAsync(CancellationToken.None);

        Assert.Single(results);
        Assert.Equal("Test Listing", results[0].Title);
    }
}
