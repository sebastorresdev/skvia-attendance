using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
using Skvia.Erp.Domain.Employees;

namespace Skvia.Erp.Application.Features.Employees.Commands.CreateEmployee;

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
            departmentId: command.DepartmentId,
            photoUrl: command.PhotoUrl,
            mainBranchId: command.MainBranchId,
            tardinessToleranceMinutes: command.TardinessToleranceMinutes);

        employee.SetAttendanceOptions(command.IsAttendanceTracked, command.AutoCompleteClockOut);
        employee.EnableMobileCheckIn(command.MobileCheckInEnabled);

        if (!string.IsNullOrWhiteSpace(command.ApplicationUserId))
        {
            employee.LinkUser(command.ApplicationUserId);
        }

        employee.SetRequireFourPointAttendance(command.RequireFourPointAttendance);
        
        if (command.AllowedWorkplaceIds != null)
        {
            employee.SetAllowedWorkplaceIds(command.AllowedWorkplaceIds);
        }

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync(cancellationToken);

        return employee.Id;
    }
}


