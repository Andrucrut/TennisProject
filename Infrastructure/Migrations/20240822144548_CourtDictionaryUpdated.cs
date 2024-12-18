using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CourtDictionaryUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "CourtDictionaries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "CourtDictionaries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteLink",
                table: "CourtDictionaries",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "CourtDictionaries");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "CourtDictionaries");

            migrationBuilder.DropColumn(
                name: "SiteLink",
                table: "CourtDictionaries");
        }
    }
}
