using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
using Skvia.Erp.Domain.Employees;

namespace Skvia.Erp.Application.Features.Employees.Commands.DeleteEmployee;

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


