using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler(IApplicationDbContext db) : ICommandHandler<UpdateEmployeeCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(UpdateEmployeeCommand command, CancellationToken cancellationToken)
    {
        var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound;
        }
        
        employee.Update(
            command.Code,
            command.FirstName,
            command.LastName,
            (DocumentType)command.DocumentType,
            command.DocumentNumber,
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
