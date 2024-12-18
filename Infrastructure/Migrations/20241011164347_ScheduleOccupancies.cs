using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ScheduleOccupancies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Expectations",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GameLevelMax",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GameLevelMin",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "TimeSlots",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "WhoPays",
                table: "Games");

            migrationBuilder.AddColumn<int>(
                name: "Go2SportId",
                table: "Courts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Go2SportLink",
                table: "CourtOrganizations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasWidget",
                table: "CourtOrganizations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ScheduleOccupancies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScheduleId = table.Column<int>(type: "integer", nullable: true),
                    ScheduleId1 = table.Column<long>(type: "bigint", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleOccupancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleOccupancies_Schedules_ScheduleId1",
                        column: x => x.ScheduleId1,
                        principalTable: "Schedules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleOccupancies_ScheduleId1",
                table: "ScheduleOccupancies",
                column: "ScheduleId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleOccupancies");

            migrationBuilder.DropColumn(
                name: "Go2SportId",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "Go2SportLink",
                table: "CourtOrganizations");

            migrationBuilder.DropColumn(
                name: "HasWidget",
                table: "CourtOrganizations");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Games",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Expectations",
                table: "Games",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GameLevelMax",
                table: "Games",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GameLevelMin",
                table: "Games",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeSlots",
                table: "Games",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WhoPays",
                table: "Games",
                type: "integer",
                nullable: true);
        }
    }
}
