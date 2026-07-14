using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace myYardSale.Infrastructure.Persistence;

public class MyYardSaleDbContextFactory : IDesignTimeDbContextFactory<MyYardSaleDbContext>
{
    public MyYardSaleDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyYardSaleDbContext>();
        optionsBuilder.UseSqlite("Data Source=myYardSale.db");

        return new MyYardSaleDbContext(optionsBuilder.Options);
    }
}
