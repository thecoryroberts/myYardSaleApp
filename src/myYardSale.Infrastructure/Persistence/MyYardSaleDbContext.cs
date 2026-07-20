using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using myYardSale.Domain.Entities;

namespace myYardSale.Infrastructure.Persistence;

public class MyYardSaleDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public MyYardSaleDbContext(DbContextOptions<MyYardSaleDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Household> Households => Set<Household>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ListingImage> ListingImages => Set<ListingImage>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
        });

        modelBuilder.Entity<IdentityRole<int>>(entity =>
        {
            entity.ToTable("Roles");
        });

        modelBuilder.Entity<IdentityUserRole<int>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        modelBuilder.Entity<IdentityRoleClaim<int>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        modelBuilder.Entity<IdentityUserToken<int>>(entity =>
        {
            entity.ToTable("UserTokens");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(2000);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(x => x.Organization)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Household>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(x => x.Event)
                .WithMany(x => x.Households)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<ListingImage>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FileName).IsRequired().HasMaxLength(500);
            entity.Property(x => x.StoragePath).IsRequired().HasMaxLength(1000);
            entity.Property(x => x.AltText).HasMaxLength(500);
            entity.HasOne(x => x.Listing)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => x.ListingId).HasDatabaseName("IX_ListingImages_ListingId");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Listing)
                .WithMany()
                .HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => x.UserId).HasDatabaseName("IX_CartItems_UserId");
            entity.HasIndex(x => new { x.ListingId, x.UserId }).HasDatabaseName("IX_CartItems_ListingId_UserId");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.Property(x => x.TotalAmount).HasPrecision(10, 2);
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => x.UserId).HasDatabaseName("IX_Orders_UserId");
            entity.HasIndex(x => x.PlacedAt).HasDatabaseName("IX_Orders_PlacedAt");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UnitPrice).HasPrecision(10, 2);
            entity.HasOne(x => x.Order)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Listing)
                .WithMany()
                .HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(x => x.OrderId).HasDatabaseName("IX_OrderItems_OrderId");
            entity.HasIndex(x => x.ListingId).HasDatabaseName("IX_OrderItems_ListingId");
        });

        modelBuilder.Entity<Listing>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(300);
            entity.Property(x => x.Description).HasMaxLength(4000);
            entity.Property(x => x.Price).HasPrecision(10, 2);
            entity.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(x => x.Household)
                .WithMany(x => x.Listings)
                .HasForeignKey(x => x.HouseholdId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Performance indexes
            entity.HasIndex(x => x.UserId).HasDatabaseName("IX_Listings_UserId");
            entity.HasIndex(x => x.CategoryId).HasDatabaseName("IX_Listings_CategoryId");
            entity.HasIndex(x => x.Status).HasDatabaseName("IX_Listings_Status");
            entity.HasIndex(x => x.CreatedAt).HasDatabaseName("IX_Listings_CreatedAt");
            entity.HasIndex(x => new { x.Status, x.CreatedAt }).HasDatabaseName("IX_Listings_Status_CreatedAt");
        });

    }
}