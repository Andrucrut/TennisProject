using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class bookingScheduleIdAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingSchedule_Bookings_BookingsId",
                table: "BookingSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingSchedule_Schedules_SchedulesId",
                table: "BookingSchedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingSchedule",
                table: "BookingSchedule");

            migrationBuilder.RenameColumn(
                name: "SchedulesId",
                table: "BookingSchedule",
                newName: "ScheduleId");

            migrationBuilder.RenameColumn(
                name: "BookingsId",
                table: "BookingSchedule",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingSchedule_SchedulesId",
                table: "BookingSchedule",
                newName: "IX_BookingSchedule_ScheduleId");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "BookingSchedule",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingSchedule",
                table: "BookingSchedule",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    IsSent = table.Column<bool>(type: "boolean", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingSchedule_BookingId",
                table: "BookingSchedule",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSchedule_Bookings_BookingId",
                table: "BookingSchedule",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSchedule_Schedules_ScheduleId",
                table: "BookingSchedule",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingSchedule_Bookings_BookingId",
                table: "BookingSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingSchedule_Schedules_ScheduleId",
                table: "BookingSchedule");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingSchedule",
                table: "BookingSchedule");

            migrationBuilder.DropIndex(
                name: "IX_BookingSchedule_BookingId",
                table: "BookingSchedule");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BookingSchedule");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "BookingSchedule",
                newName: "SchedulesId");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "BookingSchedule",
                newName: "BookingsId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingSchedule_ScheduleId",
                table: "BookingSchedule",
                newName: "IX_BookingSchedule_SchedulesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingSchedule",
                table: "BookingSchedule",
                columns: new[] { "BookingsId", "SchedulesId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSchedule_Bookings_BookingsId",
                table: "BookingSchedule",
                column: "BookingsId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSchedule_Schedules_SchedulesId",
                table: "BookingSchedule",
                column: "SchedulesId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
