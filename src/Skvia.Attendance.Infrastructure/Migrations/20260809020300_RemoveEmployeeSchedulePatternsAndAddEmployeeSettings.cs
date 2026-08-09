using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skvia.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmployeeSchedulePatternsAndAddEmployeeSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeSchedulePatterns");

            migrationBuilder.DropColumn(
                name: "require_four_point_attendance",
                table: "branches");

            migrationBuilder.AlterColumn<bool>(
                name: "require_four_point_attendance",
                table: "employees",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "allowed_kiosk_ids",
                table: "employees",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "auto_complete_clock_out",
                table: "employees",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_attendance_tracked",
                table: "employees",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "allowed_kiosk_ids",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "auto_complete_clock_out",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "is_attendance_tracked",
                table: "employees");

            migrationBuilder.AlterColumn<bool>(
                name: "require_four_point_attendance",
                table: "employees",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "require_four_point_attendance",
                table: "branches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EmployeeSchedulePatterns",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    is_work_day = table.Column<bool>(type: "boolean", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_schedule_patterns", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_schedule_patterns_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_employee_schedule_patterns_employee_id_day_of_week",
                table: "EmployeeSchedulePatterns",
                columns: new[] { "employee_id", "day_of_week" },
                unique: true);
        }
    }
}
