using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBookingFixed2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Bookings_BookingId",
                table: "Schedule");

            migrationBuilder.DropIndex(
                name: "IX_Schedule_BookingId",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Schedule");

            migrationBuilder.CreateTable(
                name: "BookingSchedule",
                columns: table => new
                {
                    BookingsId = table.Column<long>(type: "bigint", nullable: false),
                    SchedulesId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSchedule", x => new { x.BookingsId, x.SchedulesId });
                    table.ForeignKey(
                        name: "FK_BookingSchedule_Bookings_BookingsId",
                        column: x => x.BookingsId,
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
                name: "BookingId",
                table: "Schedule",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_BookingId",
                table: "Schedule",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Bookings_BookingId",
                table: "Schedule",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");
        }
    }
}
