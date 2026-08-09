using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateEmployeeCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateEmployeeCommand command, CancellationToken cancellationToken)
    {
        var normalizedCode = command.Code.Trim().ToUpperInvariant();

        if (await dbContext.Employees.AnyAsync(e => e.Code == normalizedCode, cancellationToken))
        {
            return EmployeeErrors.CodeExists(command.Code);
        }

        var documentIdentifier = DocumentIdentifier.Create(command.DocumentType, command.DocumentNumber);

        if (await dbContext.Employees.AnyAsync(e => e.DocumentIdentifier.Type == documentIdentifier.Type && e.DocumentIdentifier.Number == documentIdentifier.Number, cancellationToken))
        {
            return EmployeeErrors.DocumentExists(command.DocumentNumber);
        }

        if (!string.IsNullOrWhiteSpace(command.ApplicationUserId))
        {
            if (await dbContext.Employees.AnyAsync(e => e.ApplicationUserId == command.ApplicationUserId, cancellationToken))
            {
                return EmployeeErrors.UserAlreadyLinked;
            }
        }

        var employee = Employee.Create(
            code: command.Code,
            firstName: command.FirstName,
            lastName: command.LastName,
            documentIdentifier: documentIdentifier,
            hireDate: command.HireDate,
            email: command.Email,
            phone: command.Phone,
            position: command.Position,
            department: command.Department,
            photoUrl: command.PhotoUrl,
            mainBranchId: command.MainBranchId);

        employee.EnableMobileCheckIn(command.MobileCheckInEnabled);
        employee.LinkUser(command.ApplicationUserId);
        
        if (command.RequireFourPointAttendance.HasValue)
        {
            employee.SetRequireFourPointAttendance(command.RequireFourPointAttendance.Value);
        }

        if (command.SchedulePatterns != null && command.SchedulePatterns.Count > 0)
        {
            var patterns = command.SchedulePatterns.Select(p => 
                Skvia.Attendance.Domain.EmployeeSchedules.EmployeeSchedulePattern.Create(
                    employee.Id, p.DayOfWeek, p.IsWorkDay, p.StartTime, p.EndTime
                )).ToList();
                
            employee.SetSchedulePattern(patterns);
        }

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync(cancellationToken);

        return employee.Id;
    }
}
