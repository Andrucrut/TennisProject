using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GameHistoryCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UpdateUserId",
                table: "Games",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GamesHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    UserStatus = table.Column<int>(type: "integer", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesHistory_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_UpdateUserId",
                table: "Games",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesHistory_GameId",
                table: "GamesHistory",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_UpdateUserId",
                table: "Games",
                column: "UpdateUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_UpdateUserId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "GamesHistory");

            migrationBuilder.DropIndex(
                name: "IX_Games_UpdateUserId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "UpdateUserId",
                table: "Games");
        }
    }
}
