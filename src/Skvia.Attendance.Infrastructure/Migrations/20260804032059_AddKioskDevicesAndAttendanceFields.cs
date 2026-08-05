using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skvia.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKioskDevicesAndAttendanceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "application_user_id",
                table: "employees",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "mobile_check_in_enabled",
                table: "employees",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "time_zone_id",
                table: "branches",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "device_id",
                table: "attendances",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "latitude",
                table: "attendances",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "longitude",
                table: "attendances",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "source",
                table: "attendances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "KioskDevices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_kiosk_devices", x => x.id);
                    table.ForeignKey(
                        name: "fk_kiosk_devices_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_kiosk_devices_branch_id",
                table: "KioskDevices",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_kiosk_devices_token",
                table: "KioskDevices",
                column: "token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KioskDevices");

            migrationBuilder.DropColumn(
                name: "application_user_id",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "mobile_check_in_enabled",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "time_zone_id",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "device_id",
                table: "attendances");

            migrationBuilder.DropColumn(
                name: "latitude",
                table: "attendances");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "attendances");

            migrationBuilder.DropColumn(
                name: "source",
                table: "attendances");
        }
    }
}
