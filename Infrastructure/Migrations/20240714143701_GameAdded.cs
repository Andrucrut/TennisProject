using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GameAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GameId",
                table: "Courts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "CourtOrganizations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "CourtOrganizations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CityId = table.Column<int>(type: "integer", nullable: true),
                    DistrictId = table.Column<int>(type: "integer", nullable: true),
                    DateWithoutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeSlots = table.Column<List<string>>(type: "jsonb", nullable: true),
                    WhoPays = table.Column<int>(type: "integer", nullable: false),
                    Expectations = table.Column<int>(type: "integer", nullable: false),
                    GameLevelMin = table.Column<double>(type: "double precision", nullable: false),
                    GameLevelMax = table.Column<double>(type: "double precision", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    CourtIds = table.Column<List<int>>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Games_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    CourtId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameOrders_Courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Courts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameOrders_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameOrders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courts_GameId",
                table: "Courts",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtOrganizations_CityId",
                table: "CourtOrganizations",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtOrganizations_DistrictId",
                table: "CourtOrganizations",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_CityId",
                table: "Districts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_GameOrders_CourtId",
                table: "GameOrders",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_GameOrders_GameId",
                table: "GameOrders",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameOrders_UserId",
                table: "GameOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_CityId",
                table: "Games",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_DistrictId",
                table: "Games",
                column: "DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourtOrganizations_Cities_CityId",
                table: "CourtOrganizations",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourtOrganizations_Districts_DistrictId",
                table: "CourtOrganizations",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courts_Games_GameId",
                table: "Courts",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourtOrganizations_Cities_CityId",
                table: "CourtOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtOrganizations_Districts_DistrictId",
                table: "CourtOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Courts_Games_GameId",
                table: "Courts");

            migrationBuilder.DropTable(
                name: "GameOrders");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Courts_GameId",
                table: "Courts");

            migrationBuilder.DropIndex(
                name: "IX_CourtOrganizations_CityId",
                table: "CourtOrganizations");

            migrationBuilder.DropIndex(
                name: "IX_CourtOrganizations_DistrictId",
                table: "CourtOrganizations");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "CourtOrganizations");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "CourtOrganizations");
        }
    }
}
