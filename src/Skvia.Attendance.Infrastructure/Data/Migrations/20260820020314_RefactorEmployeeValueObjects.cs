using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skvia.Attendance.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEmployeeValueObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_justifications_employee_id_date",
                table: "justifications",
                columns: new[] { "employee_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_justifications_status",
                table: "justifications",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_justifications_status_date",
                table: "justifications",
                columns: new[] { "status", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_employees_application_user_id",
                table: "employees",
                column: "application_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employees_status",
                table: "employees",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_justifications_employee_id_date",
                table: "justifications");

            migrationBuilder.DropIndex(
                name: "ix_justifications_status",
                table: "justifications");

            migrationBuilder.DropIndex(
                name: "ix_justifications_status_date",
                table: "justifications");

            migrationBuilder.DropIndex(
                name: "ix_employees_application_user_id",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "ix_employees_status",
                table: "employees");
        }
    }
}
