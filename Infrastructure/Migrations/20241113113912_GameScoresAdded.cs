using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GameScoresAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Results",
                table: "GameResults");

            migrationBuilder.CreateTable(
                name: "GameScores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<long>(type: "bigint", nullable: true),
                    CreatorId = table.Column<long>(type: "bigint", nullable: true),
                    OpponentId = table.Column<long>(type: "bigint", nullable: true),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    CreatorScore = table.Column<int>(type: "integer", nullable: false),
                    OpponentScore = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameScores_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameScores_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameScores_Users_OpponentId",
                        column: x => x.OpponentId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameScoreResult",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameResultId = table.Column<long>(type: "bigint", nullable: false),
                    GameScoreId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameScoreResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameScoreResult_GameResults_GameResultId",
                        column: x => x.GameResultId,
                        principalTable: "GameResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameScoreResult_GameScores_GameScoreId",
                        column: x => x.GameScoreId,
                        principalTable: "GameScores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameScoreResult_GameResultId",
                table: "GameScoreResult",
                column: "GameResultId");

            migrationBuilder.CreateIndex(
                name: "IX_GameScoreResult_GameScoreId",
                table: "GameScoreResult",
                column: "GameScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_GameScores_CreatorId",
                table: "GameScores",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_GameScores_GameId",
                table: "GameScores",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameScores_OpponentId",
                table: "GameScores",
                column: "OpponentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameScoreResult");

            migrationBuilder.DropTable(
                name: "GameScores");

            migrationBuilder.AddColumn<string>(
                name: "Results",
                table: "GameResults",
                type: "jsonb",
                nullable: true);
        }
    }
}
