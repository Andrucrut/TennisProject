using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Schedule_PriceJsondAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Schedules");

            migrationBuilder.AddColumn<string>(
                name: "PriceJson",
                table: "Schedules",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceJson",
                table: "Schedules");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Schedules",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
