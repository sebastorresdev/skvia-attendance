using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Features.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<DeleteEmployeeCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteEmployeeCommand command, CancellationToken cancellationToken)
    {
        var affectedRows = await dbContext.Employees
            .Where (e => e.Id == command.EmployeeId)
            .ExecuteDeleteAsync(cancellationToken);
        
        return affectedRows > 0 ? Result.Success : EmployeeErrors.NotFound;
    }
}
