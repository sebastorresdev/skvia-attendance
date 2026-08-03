using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.Employees;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Skvia.Attendance.Application.Features.Employees.Commands.ChangeEmployeeStatus;

public class ChangeEmployeeStatusCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<ChangeEmployeeStatusCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(ChangeEmployeeStatusCommand command, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == command.EmployeeId, cancellationToken);

        if (employee is null)
            return EmployeeErrors.NotFound;

        employee.ChangeStatus(command.NewStatus);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success;
    }
}
