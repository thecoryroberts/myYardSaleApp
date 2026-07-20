using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace myYardSale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PickupNotes",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupNotes",
                table: "Users");
        }
    }
}
