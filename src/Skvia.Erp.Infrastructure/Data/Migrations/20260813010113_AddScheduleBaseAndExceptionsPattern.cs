using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skvia.Erp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleBaseAndExceptionsPattern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_employee_schedules_employee_id_date",
                table: "employee_schedules");

            migrationBuilder.AddColumn<DateOnly>(
                name: "effective_from",
                table: "employee_schedules",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "effective_to",
                table: "employee_schedules",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "schedule_id",
                table: "employee_schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "schedule_exceptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    custom_schedule_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_day_off = table.Column<bool>(type: "boolean", nullable: false),
                    day_type = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    reason = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schedule_exceptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_schedule_exceptions_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_schedule_exceptions_schedules_custom_schedule_id",
                        column: x => x.custom_schedule_id,
                        principalTable: "schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_employee_schedules_employee_id_effective_from",
                table: "employee_schedules",
                columns: new[] { "employee_id", "effective_from" });

            migrationBuilder.CreateIndex(
                name: "ix_employee_schedules_schedule_id",
                table: "employee_schedules",
                column: "schedule_id");

            migrationBuilder.CreateIndex(
                name: "ix_schedule_exceptions_custom_schedule_id",
                table: "schedule_exceptions",
                column: "custom_schedule_id");

            migrationBuilder.CreateIndex(
                name: "ix_schedule_exceptions_employee_id_date",
                table: "schedule_exceptions",
                columns: new[] { "employee_id", "date" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_schedules_schedules_schedule_id",
                table: "employee_schedules",
                column: "schedule_id",
                principalTable: "schedules",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_employee_schedules_schedules_schedule_id",
                table: "employee_schedules");

            migrationBuilder.DropTable(
                name: "schedule_exceptions");

            migrationBuilder.DropIndex(
                name: "ix_employee_schedules_employee_id_effective_from",
                table: "employee_schedules");

            migrationBuilder.DropIndex(
                name: "ix_employee_schedules_schedule_id",
                table: "employee_schedules");

            migrationBuilder.DropColumn(
                name: "effective_from",
                table: "employee_schedules");

            migrationBuilder.DropColumn(
                name: "effective_to",
                table: "employee_schedules");

            migrationBuilder.DropColumn(
                name: "schedule_id",
                table: "employee_schedules");

            migrationBuilder.CreateIndex(
                name: "ix_employee_schedules_employee_id_date",
                table: "employee_schedules",
                columns: new[] { "employee_id", "date" },
                unique: true);
        }
    }
}

