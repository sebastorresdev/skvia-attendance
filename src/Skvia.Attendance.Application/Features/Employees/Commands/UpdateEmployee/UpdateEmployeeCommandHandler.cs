using Skvia.Attendance.Domain.Employees;
using Microsoft.EntityFrameworkCore;

namespace Skvia.Attendance.Application.Features.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler(IApplicationDbContext db) : ICommandHandler<UpdateEmployeeCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(UpdateEmployeeCommand command, CancellationToken cancellationToken)
    {
        var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound;
        }

        var documentIdentifier = DocumentIdentifier.Create(command.DocumentType, command.DocumentNumber);

        // Check for duplicate document number if it's being changed and belongs to another employee
        if (employee.DocumentIdentifier.Type != documentIdentifier.Type || employee.DocumentIdentifier.Number != documentIdentifier.Number)
        {
            if (await db.Employees.AnyAsync(e => e.DocumentIdentifier.Type == documentIdentifier.Type && e.DocumentIdentifier.Number == documentIdentifier.Number && e.Id != command.Id, cancellationToken))
            {
                return EmployeeErrors.DocumentExists(command.DocumentNumber);
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
            command.PhotoUrl);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
