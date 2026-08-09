using Skvia.Attendance.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<UpdateEmployeeCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(UpdateEmployeeCommand command, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .Include(e => e.SchedulePatterns)
            .FirstOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound;
        }

        var documentIdentifier = DocumentIdentifier.Create(command.DocumentType, command.DocumentNumber);

        // Check for duplicate document number if it's being changed and belongs to another employee
        if (employee.DocumentIdentifier.Type != documentIdentifier.Type || employee.DocumentIdentifier.Number != documentIdentifier.Number)
        {
            if (await dbContext.Employees.AnyAsync(e => e.DocumentIdentifier.Type == documentIdentifier.Type && e.DocumentIdentifier.Number == documentIdentifier.Number && e.Id != command.Id, cancellationToken))
            {
                return EmployeeErrors.DocumentExists(command.DocumentNumber);
            }
        }

        if (!string.IsNullOrWhiteSpace(command.ApplicationUserId) && employee.ApplicationUserId != command.ApplicationUserId)
        {
            if (await dbContext.Employees.AnyAsync(e => e.ApplicationUserId == command.ApplicationUserId && e.Id != command.Id, cancellationToken))
            {
                return EmployeeErrors.UserAlreadyLinked;
            }
        }

        employee.Update(
            command.Code,
            command.FirstName,
            command.LastName,
            documentIdentifier,
            command.HireDate,
            command.Email,
            command.Phone,
            command.Position,
            command.Department,
            command.PhotoUrl,
            command.MainBranchId);

        employee.EnableMobileCheckIn(command.MobileCheckInEnabled);
        employee.LinkUser(command.ApplicationUserId);
        employee.SetRequireFourPointAttendance(command.RequireFourPointAttendance);

        if (command.SchedulePatterns != null)
        {
            var patterns = command.SchedulePatterns.Select(p => 
                Skvia.Attendance.Domain.EmployeeSchedules.EmployeeSchedulePattern.Create(
                    employee.Id, p.DayOfWeek, p.IsWorkDay, p.StartTime, p.EndTime
                )).ToList();
                
            employee.SetSchedulePattern(patterns);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
