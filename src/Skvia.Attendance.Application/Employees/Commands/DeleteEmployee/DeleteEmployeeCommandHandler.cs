using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler(IApplicationDbContext db) : ICommandHandler<DeleteEmployeeCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteEmployeeCommand command, CancellationToken cancellationToken)
    {
        var affectedRows = await db.Employees
            .Where (e => e.Id == command.EmployeeId)
            .ExecuteDeleteAsync(cancellationToken);
        
        return affectedRows > 0 ? Result.Success : EmployeeErrors.NotFound;
    }
}
