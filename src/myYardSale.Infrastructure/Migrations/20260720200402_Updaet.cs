using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace myYardSale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updaet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_ListingId",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PlacedAt",
                table: "Orders",
                column: "PlacedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_CreatedAt",
                table: "Listings",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_Status",
                table: "Listings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_Status_CreatedAt",
                table: "Listings",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ListingId_UserId",
                table: "CartItems",
                columns: new[] { "ListingId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_PlacedAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Listings_CreatedAt",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_Status",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_Status_CreatedAt",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ListingId_UserId",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ListingId",
                table: "CartItems",
                column: "ListingId");
        }
    }
}
