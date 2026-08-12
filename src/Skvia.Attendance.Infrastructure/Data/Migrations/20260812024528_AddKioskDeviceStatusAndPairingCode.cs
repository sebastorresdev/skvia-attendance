using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skvia.Attendance.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddKioskDeviceStatusAndPairingCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "KioskDevices");

            migrationBuilder.AddColumn<DateTime>(
                name: "linked_at",
                table: "KioskDevices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pairing_code",
                table: "KioskDevices",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "pairing_code_expires_at",
                table: "KioskDevices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "KioskDevices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_kiosk_devices_pairing_code",
                table: "KioskDevices",
                column: "pairing_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_kiosk_devices_pairing_code",
                table: "KioskDevices");

            migrationBuilder.DropColumn(
                name: "linked_at",
                table: "KioskDevices");

            migrationBuilder.DropColumn(
                name: "pairing_code",
                table: "KioskDevices");

            migrationBuilder.DropColumn(
                name: "pairing_code_expires_at",
                table: "KioskDevices");

            migrationBuilder.DropColumn(
                name: "status",
                table: "KioskDevices");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "KioskDevices",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
