using Skvia.Attendance.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Domain.Attendances;
using Skvia.Attendance.Domain.Common;

namespace Skvia.Attendance.Api.Endpoints.Attendances;

public sealed class SeedAttendances : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/seed", Handle)
            .WithName(nameof(SeedAttendances))
            .WithSummary("Generar asistencias de prueba (DEV ONLY)")
            .WithDescription("Genera datos de asistencia aleatorios para los últimos 30 días.")
            .Produces(StatusCodes.Status200OK);

    private static async Task<IResult> Handle(
        IApplicationDbContext dbContext,
        IClock clock,
        ITimeZoneProvider timeZoneProvider,
        CancellationToken cancellationToken)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(cancellationToken);
        if (branch is null) return TypedResults.Ok("No hay sedes para generar datos.");

        var employees = await dbContext.Employees.Where(e => e.Status == Domain.Employees.EmployeeStatus.Active).ToListAsync(cancellationToken);
        if (!employees.Any())
        {
            var emp1 = Domain.Employees.Employee.Create(
                "EMP001", "Juan", "Pérez",
                Domain.Employees.DocumentIdentifier.Create(Domain.Employees.DocumentType.Dni, "70123456"),
                DateTimeOffset.UtcNow, "juan.perez@skvia.pe", "987654321", "Desarrollador Senior", "TI", null, branch.Id);

            var emp2 = Domain.Employees.Employee.Create(
                "EMP002", "María", "Gómez",
                Domain.Employees.DocumentIdentifier.Create(Domain.Employees.DocumentType.Dni, "70654321"),
                DateTimeOffset.UtcNow, "maria.gomez@skvia.pe", "987123456", "Analista de RRHH", "RRHH", null, branch.Id);

            var emp3 = Domain.Employees.Employee.Create(
                "EMP003", "Carlos", "López",
                Domain.Employees.DocumentIdentifier.Create(Domain.Employees.DocumentType.Dni, "70987654"),
                DateTimeOffset.UtcNow, "carlos.lopez@skvia.pe", "987999888", "Soporte Técnico", "TI", null, branch.Id);

            dbContext.Employees.AddRange(emp1, emp2, emp3);
            await dbContext.SaveChangesAsync(cancellationToken);
            employees = new List<Domain.Employees.Employee> { emp1, emp2, emp3 };
        }

        var localTime = TimeZoneInfo.ConvertTime(clock.UtcNow, timeZoneProvider.GetTimeZone(branch.TimeZoneId));
        var today = DateOnly.FromDateTime(localTime.DateTime);
        var random = new Random();
        int createdCount = 0;

        for (int i = 0; i <= 30; i++)
        {
            var date = today.AddDays(-i);
            // Skip weekends for simplicity
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;

            foreach (var employee in employees)
            {
                // 80% chance of attendance
                if (random.Next(100) < 80)
                {
                    var assignedStart = new TimeOnly(9, 0, 0); // 9:00 AM
                    
                    // Generate a CheckIn time between 8:30 AM and 9:30 AM
                    int randomMinutes = random.Next(-30, 30);
                    var checkInTime = new DateTime(date.Year, date.Month, date.Day, 9, 0, 0).AddMinutes(randomMinutes);
                    // Convert to UTC for saving
                    var checkInUtc = TimeZoneInfo.ConvertTimeToUtc(checkInTime, timeZoneProvider.GetTimeZone(branch.TimeZoneId));

                    var attendance = Skvia.Attendance.Domain.Attendances.Attendance.CreateCheckIn(
                        employee.Id,
                        branch.Id,
                        "mock.jpg",
                        true,
                        assignedStart,
                        branch.TimeZoneId,
                        clock, // not used directly in this mock block
                        timeZoneProvider,
                        AttendanceSource.Kiosk,
                        null, null, "MockDevice",
                        branch.TardinessToleranceMinutes);

                    // Reflection or setting backing fields isn't easy here, 
                    // Let's just use EF Core reflection to set private CheckIn date for the mock
                    var checkInProperty = typeof(Skvia.Attendance.Domain.Attendances.Attendance).GetProperty(nameof(attendance.CheckIn));
                    checkInProperty?.SetValue(attendance, new DateTimeOffset(checkInUtc, TimeSpan.Zero));
                    
                    var dateProperty = typeof(Skvia.Attendance.Domain.Attendances.Attendance).GetProperty(nameof(attendance.Date));
                    dateProperty?.SetValue(attendance, date);

                    // 90% chance they checked out
                    if (random.Next(100) < 90)
                    {
                        var checkOutTime = new DateTime(date.Year, date.Month, date.Day, 18, 0, 0).AddMinutes(random.Next(-10, 60)); // 18:00 to 19:00
                        var checkOutUtc = TimeZoneInfo.ConvertTimeToUtc(checkOutTime, timeZoneProvider.GetTimeZone(branch.TimeZoneId));

                        var checkOutProperty = typeof(Skvia.Attendance.Domain.Attendances.Attendance).GetProperty(nameof(attendance.CheckOut));
                        checkOutProperty?.SetValue(attendance, new DateTimeOffset(checkOutUtc, TimeSpan.Zero));
                    }

                    dbContext.Attendances.Add(attendance);
                    createdCount++;
                }
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok($"Se generaron {createdCount} registros de asistencia aleatorios.");
    }
}
