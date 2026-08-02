using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.EmployeeSchedules;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Domain.Branches;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.AssignWeeklySchedule;

public class AssignWeeklyScheduleCommandHandler(
    IEmployeeScheduleRepository scheduleRepository,
    IApplicationDbContext context) : ICommandHandler<AssignWeeklyScheduleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(AssignWeeklyScheduleCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que el empleado exista
        var employeeExists = await context.Employees.AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);
        if (!employeeExists)
            return Error.NotFound(description: "Empleado no encontrado.");

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
            // Validar si la sucursal existe si no es un día libre/vacaciones que tal vez no envíe sucursal
            // En Retail, hasta los días libres suelen asignarse a la sucursal base, pero por seguridad verificamos:
            if (day.BranchId != Guid.Empty)
            {
                var branchExists = await context.Branches.AnyAsync(b => b.Id == day.BranchId, cancellationToken);
                if (!branchExists)
                    return Error.NotFound(description: $"Sucursal no encontrada para el día {day.Date}.");
            }

            ErrorOr<EmployeeSchedule> scheduleResult = day.DayType switch
            {
                ScheduleDayType.WorkDay => EmployeeSchedule.CreateWorkDay(request.EmployeeId, day.Date, day.BranchId, day.StartTime!.Value, day.EndTime!.Value, day.BaseScheduleId),
                ScheduleDayType.DayOff => EmployeeSchedule.CreateRestDay(request.EmployeeId, day.Date, day.BranchId),
                ScheduleDayType.Vacation => EmployeeSchedule.CreateVacationDay(request.EmployeeId, day.Date, day.BranchId),
                ScheduleDayType.MedicalLeave => EmployeeSchedule.CreateMedicalLeaveDay(request.EmployeeId, day.Date, day.BranchId),
                ScheduleDayType.MakeUpDay => EmployeeSchedule.CreateMakeUpDay(request.EmployeeId, day.Date, day.BranchId, day.StartTime!.Value, day.EndTime!.Value, day.BaseScheduleId),
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
