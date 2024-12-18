using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CourtType_HasAdditionalServices_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleOccupancies_Schedules_ScheduleId1",
                table: "ScheduleOccupancies");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleOccupancies_ScheduleId1",
                table: "ScheduleOccupancies");

            migrationBuilder.DropColumn(
                name: "ScheduleId1",
                table: "ScheduleOccupancies");

            migrationBuilder.AlterColumn<long>(
                name: "ScheduleId",
                table: "ScheduleOccupancies",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourtType",
                table: "Courts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasAdditionalServices",
                table: "CourtOrganizations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleOccupancies_ScheduleId",
                table: "ScheduleOccupancies",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleOccupancies_Schedules_ScheduleId",
                table: "ScheduleOccupancies",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleOccupancies_Schedules_ScheduleId",
                table: "ScheduleOccupancies");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleOccupancies_ScheduleId",
                table: "ScheduleOccupancies");

            migrationBuilder.DropColumn(
                name: "CourtType",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "HasAdditionalServices",
                table: "CourtOrganizations");

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleId",
                table: "ScheduleOccupancies",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ScheduleId1",
                table: "ScheduleOccupancies",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleOccupancies_ScheduleId1",
                table: "ScheduleOccupancies",
                column: "ScheduleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleOccupancies_Schedules_ScheduleId1",
                table: "ScheduleOccupancies",
                column: "ScheduleId1",
                principalTable: "Schedules",
                principalColumn: "Id");
        }
    }
}
