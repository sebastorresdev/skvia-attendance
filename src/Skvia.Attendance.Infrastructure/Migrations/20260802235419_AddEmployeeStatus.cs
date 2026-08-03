using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skvia.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_employee_schedules_employees_employee_id",
                table: "employee_schedules");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "employees",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_schedules_employees_employee_id",
                table: "employee_schedules",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_employee_schedules_employees_employee_id",
                table: "employee_schedules");

            migrationBuilder.DropColumn(
                name: "status",
                table: "employees");

            migrationBuilder.AddForeignKey(
                name: "fk_employee_schedules_employees_employee_id",
                table: "employee_schedules",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
