using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourtDictionaryId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CourtDictionaryId",
                table: "Users",
                column: "CourtDictionaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_CourtDictionaries_CourtDictionaryId",
                table: "Users",
                column: "CourtDictionaryId",
                principalTable: "CourtDictionaries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_CourtDictionaries_CourtDictionaryId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CourtDictionaryId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CourtDictionaryId",
                table: "Users");
        }
    }
}
