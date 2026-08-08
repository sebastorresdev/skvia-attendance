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
        employee.SetRequireFourPointAttendance(command.RequireFourPointAttendance);

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync(cancellationToken);

        return employee.Id;
    }
}
