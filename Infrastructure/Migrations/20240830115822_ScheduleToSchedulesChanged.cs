using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ScheduleToSchedulesChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingSchedule_Schedule_SchedulesId",
                table: "BookingSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Courts_CourtId",
                table: "Schedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule");

            migrationBuilder.RenameTable(
                name: "Schedule",
                newName: "Schedules");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_CourtId",
                table: "Schedules",
                newName: "IX_Schedules_CourtId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSchedule_Schedules_SchedulesId",
                table: "BookingSchedule",
                column: "SchedulesId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Courts_CourtId",
                table: "Schedules",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingSchedule_Schedules_SchedulesId",
                table: "BookingSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Courts_CourtId",
                table: "Schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules");

            migrationBuilder.RenameTable(
                name: "Schedules",
                newName: "Schedule");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_CourtId",
                table: "Schedule",
                newName: "IX_Schedule_CourtId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSchedule_Schedule_SchedulesId",
                table: "BookingSchedule",
                column: "SchedulesId",
                principalTable: "Schedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Courts_CourtId",
                table: "Schedule",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "Id");
        }
    }
}
