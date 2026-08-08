using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skvia.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MobileAttendanceConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "require_four_point_attendance",
                table: "employees",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "geofence_radius_meters",
                table: "branches",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "latitude",
                table: "branches",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "longitude",
                table: "branches",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "require_four_point_attendance",
                table: "branches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "require_photo_for_mobile",
                table: "branches",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "require_four_point_attendance",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "geofence_radius_meters",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "latitude",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "require_four_point_attendance",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "require_photo_for_mobile",
                table: "branches");
        }
    }
}
