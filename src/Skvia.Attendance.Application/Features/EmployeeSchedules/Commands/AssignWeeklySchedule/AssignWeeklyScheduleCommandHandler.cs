using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.AssignWeeklySchedule;

public class AssignWeeklyScheduleCommandHandler(
    IEmployeeScheduleRepository scheduleRepository,
    IApplicationDbContext context) : ICommandHandler<AssignWeeklyScheduleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(AssignWeeklyScheduleCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que el empleado exista
        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);
        if (employee is null)
            return Error.NotFound(description: "Empleado no encontrado.");

        if (!employee.IsAttendanceTracked)
            return Error.Validation(description: "No se puede asignar horarios a un empleado que no tiene activado el control de asistencia.");

        var hireDateOnly = DateOnly.FromDateTime(employee.HireDate.Date);

        // 2. Obtener horarios existentes para esta semana y eliminarlos
        var existingSchedules = await scheduleRepository.GetByEmployeeAndDateRangeAsync(
            request.EmployeeId, request.StartDate, request.EndDate, cancellationToken);
            
        if (existingSchedules.Count > 0)
        {
            await scheduleRepository.DeleteRangeAsync(existingSchedules, cancellationToken);
        }

        // 3. Crear los nuevos horarios
        var newSchedules = new List<EmployeeSchedule>();
        foreach (var day in request.Days)
        {
            if (day.Date < hireDateOnly)
            {
                return Error.Validation("Employee.InvalidScheduleDate",
                    $"No se puede programar horario para la fecha {day.Date:dd/MM/yyyy} porque es anterior a la fecha de ingreso del empleado ({hireDateOnly:dd/MM/yyyy}).");
            }
            
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (day.Date < today)
            {
                return Error.Validation("Employee.InvalidScheduleDate",
                    $"No se puede programar horario para fechas pasadas ({day.Date:dd/MM/yyyy}).");
            }

            ErrorOr<EmployeeSchedule> scheduleResult = day.DayType switch
            {
                ScheduleDayType.WorkDay => EmployeeSchedule.CreateWorkDay(request.EmployeeId, day.Date, day.StartTime!.Value, day.EndTime!.Value, day.BaseScheduleId),
                ScheduleDayType.DayOff => EmployeeSchedule.CreateRestDay(request.EmployeeId, day.Date),
                ScheduleDayType.Vacation => EmployeeSchedule.CreateVacationDay(request.EmployeeId, day.Date),
                ScheduleDayType.MedicalLeave => EmployeeSchedule.CreateMedicalLeaveDay(request.EmployeeId, day.Date),
                ScheduleDayType.MakeUpDay => EmployeeSchedule.CreateMakeUpDay(request.EmployeeId, day.Date, day.StartTime!.Value, day.EndTime!.Value, day.BaseScheduleId),
                _ => Error.Validation(description: "Tipo de día no válido.")
            };

            if (scheduleResult.IsError)
                return scheduleResult.Errors;

            newSchedules.Add(scheduleResult.Value);
        }

        await scheduleRepository.AddRangeAsync(newSchedules, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
