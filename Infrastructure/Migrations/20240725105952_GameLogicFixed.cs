using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GameLogicFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courts_Games_GameId",
                table: "Courts");

            migrationBuilder.DropForeignKey(
                name: "FK_GameOrders_Courts_CourtId",
                table: "GameOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Cities_CityId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Districts_DistrictId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_CityId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_GameOrders_CourtId",
                table: "GameOrders");

            migrationBuilder.DropIndex(
                name: "IX_Courts_GameId",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CourtIds",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CourtId",
                table: "GameOrders");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Courts");

            migrationBuilder.RenameColumn(
                name: "DistrictId",
                table: "Games",
                newName: "CourtOrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_DistrictId",
                table: "Games",
                newName: "IX_Games_CourtOrganizationId");

            migrationBuilder.AlterColumn<int>(
                name: "WhoPays",
                table: "Games",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "GameLevelMin",
                table: "Games",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "GameLevelMax",
                table: "Games",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "Expectations",
                table: "Games",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Games",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BookingId",
                table: "GameOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Courts",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_GameOrders_BookingId",
                table: "GameOrders",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameOrders_Bookings_BookingId",
                table: "GameOrders",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_CourtOrganizations_CourtOrganizationId",
                table: "Games",
                column: "CourtOrganizationId",
                principalTable: "CourtOrganizations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameOrders_Bookings_BookingId",
                table: "GameOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_CourtOrganizations_CourtOrganizationId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_GameOrders_BookingId",
                table: "GameOrders");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "GameOrders");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "CourtOrganizationId",
                table: "Games",
                newName: "DistrictId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_CourtOrganizationId",
                table: "Games",
                newName: "IX_Games_DistrictId");

            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "Schedule",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartTime",
                table: "Schedule",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "WhoPays",
                table: "Games",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "GameLevelMin",
                table: "Games",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "GameLevelMax",
                table: "Games",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Expectations",
                table: "Games",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Games",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<List<int>>(
                name: "CourtIds",
                table: "Games",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourtId",
                table: "GameOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GameId",
                table: "Courts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_CityId",
                table: "Games",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_GameOrders_CourtId",
                table: "GameOrders",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_Courts_GameId",
                table: "Courts",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courts_Games_GameId",
                table: "Courts",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameOrders_Courts_CourtId",
                table: "GameOrders",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Cities_CityId",
                table: "Games",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Districts_DistrictId",
                table: "Games",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id");
        }
    }
}
