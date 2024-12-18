using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable
    
namespace Infrastructure.Migrations
{   
    /// <inheritdoc />
    public partial class ReviewsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourtReviews",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourtOrganizationId = table.Column<int>(type: "integer", nullable: true),
                    ReviewerId = table.Column<long>(type: "bigint", nullable: true),
                    GameId = table.Column<long>(type: "bigint", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Disappointments = table.Column<int[]>(type: "integer[]", nullable: true),
                    Satisfactions = table.Column<int[]>(type: "integer[]", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourtReviews_CourtOrganizations_CourtOrganizationId",
                        column: x => x.CourtOrganizationId,
                        principalTable: "CourtOrganizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourtReviews_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourtReviews_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserReviews",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewerId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedPlayerId = table.Column<long>(type: "bigint", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReviews_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserReviews_Users_ReviewedPlayerId",
                        column: x => x.ReviewedPlayerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserReviews_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourtReviews_CourtOrganizationId",
                table: "CourtReviews",
                column: "CourtOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtReviews_GameId",
                table: "CourtReviews",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtReviews_ReviewerId",
                table: "CourtReviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_GameId",
                table: "UserReviews",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_ReviewedPlayerId",
                table: "UserReviews",
                column: "ReviewedPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_ReviewerId",
                table: "UserReviews",
                column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourtReviews");

            migrationBuilder.DropTable(
                name: "UserReviews");
        }
    }
}
