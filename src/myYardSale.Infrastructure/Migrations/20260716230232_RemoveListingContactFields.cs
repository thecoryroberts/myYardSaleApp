using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace myYardSale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveListingContactFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ContactNotes",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Listings");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNotes",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ContactNotes",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Listings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "Listings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNotes",
                table: "Listings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Listings",
                type: "TEXT",
                nullable: true);
        }
    }
}
