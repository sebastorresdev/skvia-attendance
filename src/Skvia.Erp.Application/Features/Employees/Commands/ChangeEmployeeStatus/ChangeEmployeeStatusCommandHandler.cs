using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Common;
using Skvia.Erp.Domain.Employees;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Skvia.Erp.Application.Features.Employees.Commands.ChangeEmployeeStatus;

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


