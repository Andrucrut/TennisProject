using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Schedule_ScheduleId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ScheduleId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Bookings");

            migrationBuilder.CreateTable(
                name: "BookingSchedule",
                columns: table => new
                {
                    BookingId = table.Column<long>(type: "bigint", nullable: false),
                    SchedulesId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSchedule", x => new { x.BookingId, x.SchedulesId });
                    table.ForeignKey(
                        name: "FK_BookingSchedule_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingSchedule_Schedule_SchedulesId",
                        column: x => x.SchedulesId,
                        principalTable: "Schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingSchedule_SchedulesId",
                table: "BookingSchedule",
                column: "SchedulesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingSchedule");

            migrationBuilder.AddColumn<long>(
                name: "ScheduleId",
                table: "Bookings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ScheduleId",
                table: "Bookings",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Schedule_ScheduleId",
                table: "Bookings",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id");
        }
    }
}
