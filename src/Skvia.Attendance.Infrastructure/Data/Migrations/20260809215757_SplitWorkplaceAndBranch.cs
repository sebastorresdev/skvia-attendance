using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skvia.Attendance.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SplitWorkplaceAndBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attendances_branches_check_in_branch_id",
                table: "attendances");

            migrationBuilder.DropForeignKey(
                name: "fk_attendances_branches_check_out_branch_id",
                table: "attendances");

            migrationBuilder.DropForeignKey(
                name: "fk_kiosk_devices_branches_branch_id",
                table: "KioskDevices");

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
                name: "require_photo_for_mobile",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "tardiness_tolerance_minutes",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "time_zone_id",
                table: "branches");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "KioskDevices",
                newName: "workplace_id");

            migrationBuilder.RenameIndex(
                name: "ix_kiosk_devices_branch_id",
                table: "KioskDevices",
                newName: "ix_kiosk_devices_workplace_id");

            migrationBuilder.RenameColumn(
                name: "allowed_kiosk_ids",
                table: "employees",
                newName: "allowed_workplace_ids");

            migrationBuilder.RenameColumn(
                name: "check_out_branch_id",
                table: "attendances",
                newName: "check_out_workplace_id");

            migrationBuilder.RenameColumn(
                name: "check_in_branch_id",
                table: "attendances",
                newName: "check_in_workplace_id");

            migrationBuilder.RenameIndex(
                name: "ix_attendances_check_out_branch_id_date",
                table: "attendances",
                newName: "ix_attendances_check_out_workplace_id_date");

            migrationBuilder.RenameIndex(
                name: "ix_attendances_check_in_branch_id_date",
                table: "attendances",
                newName: "ix_attendances_check_in_workplace_id_date");

            migrationBuilder.AddColumn<int>(
                name: "tardiness_tolerance_minutes",
                table: "employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "workplaces",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    time_zone_id = table.Column<string>(type: "text", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    geofence_radius_meters = table.Column<double>(type: "double precision", nullable: false),
                    require_photo_for_mobile = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workplaces", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_workplaces_code",
                table: "workplaces",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_attendances_workplaces_check_in_workplace_id",
                table: "attendances",
                column: "check_in_workplace_id",
                principalTable: "workplaces",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_attendances_workplaces_check_out_workplace_id",
                table: "attendances",
                column: "check_out_workplace_id",
                principalTable: "workplaces",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_kiosk_devices_workplaces_workplace_id",
                table: "KioskDevices",
                column: "workplace_id",
                principalTable: "workplaces",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attendances_workplaces_check_in_workplace_id",
                table: "attendances");

            migrationBuilder.DropForeignKey(
                name: "fk_attendances_workplaces_check_out_workplace_id",
                table: "attendances");

            migrationBuilder.DropForeignKey(
                name: "fk_kiosk_devices_workplaces_workplace_id",
                table: "KioskDevices");

            migrationBuilder.DropTable(
                name: "workplaces");

            migrationBuilder.DropColumn(
                name: "tardiness_tolerance_minutes",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "workplace_id",
                table: "KioskDevices",
                newName: "branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_kiosk_devices_workplace_id",
                table: "KioskDevices",
                newName: "ix_kiosk_devices_branch_id");

            migrationBuilder.RenameColumn(
                name: "allowed_workplace_ids",
                table: "employees",
                newName: "allowed_kiosk_ids");

            migrationBuilder.RenameColumn(
                name: "check_out_workplace_id",
                table: "attendances",
                newName: "check_out_branch_id");

            migrationBuilder.RenameColumn(
                name: "check_in_workplace_id",
                table: "attendances",
                newName: "check_in_branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_attendances_check_out_workplace_id_date",
                table: "attendances",
                newName: "ix_attendances_check_out_branch_id_date");

            migrationBuilder.RenameIndex(
                name: "ix_attendances_check_in_workplace_id_date",
                table: "attendances",
                newName: "ix_attendances_check_in_branch_id_date");

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
                name: "require_photo_for_mobile",
                table: "branches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "tardiness_tolerance_minutes",
                table: "branches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "time_zone_id",
                table: "branches",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "fk_attendances_branches_check_in_branch_id",
                table: "attendances",
                column: "check_in_branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_attendances_branches_check_out_branch_id",
                table: "attendances",
                column: "check_out_branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_kiosk_devices_branches_branch_id",
                table: "KioskDevices",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
