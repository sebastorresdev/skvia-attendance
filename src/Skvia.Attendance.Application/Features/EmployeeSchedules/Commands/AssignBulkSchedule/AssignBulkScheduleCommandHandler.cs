using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.AssignBulkSchedule;

public class AssignBulkScheduleCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<AssignBulkScheduleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(AssignBulkScheduleCommand request, CancellationToken cancellationToken)
    {
        if (request.EmployeeIds.Count == 0)
            return Error.Validation("AssignBulkSchedule.NoEmployees", "Debe seleccionar al menos un empleado.");

        // Validar plantilla existente
        var scheduleTemplate = await dbContext.Schedules
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.ScheduleTemplateId, cancellationToken);

        if (scheduleTemplate is null)
            return Error.NotFound("Schedule.NotFound", "La plantilla de horario no existe.");

        // Validar empleados existentes
        var employees = await dbContext.Employees
            .Where(e => request.EmployeeIds.Contains(e.Id))
            .ToListAsync(cancellationToken);

        if (employees.Count == 0)
            return Error.NotFound("Employees.NotFound", "No se encontraron empleados válidos.");

        // Crear asignaciones masivas
        var newAssignments = new List<EmployeeSchedule>();

        foreach (var employee in employees)
        {
            var assignmentResult = EmployeeSchedule.CreateAssignment(
                employee.Id,
                scheduleTemplate.Id,
                request.EffectiveFrom,
                request.EffectiveTo);

            if (assignmentResult.IsError)
                return assignmentResult.Errors;

            newAssignments.Add(assignmentResult.Value);
        }

        await dbContext.EmployeeSchedules.AddRangeAsync(newAssignments, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
