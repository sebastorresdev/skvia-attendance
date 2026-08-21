using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Features.Employees.DTOs;

namespace Skvia.Erp.Application.Features.Employees.Queries.GetEmployees;

/// <summary>
/// Manejador para la consulta de obtención del listado de empleados.
/// </summary>
public class GetEmployeesQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetEmployeesQuery, ErrorOr<List<EmployeeResponse>>>
{
    public async Task<ErrorOr<List<EmployeeResponse>>> HandleAsync(GetEmployeesQuery query, CancellationToken cancellationToken)
    {
        var employees = await dbContext.Employees.AsNoTracking()
            .OrderBy(e => e.Code)
            .GroupJoin(
                dbContext.Branches.AsNoTracking(),
                e => e.MainBranchId,
                b => b.Id,
                (e, branchGroup) => new { Employee = e, BranchGroup = branchGroup }
            )
            .SelectMany(
                x => x.BranchGroup.DefaultIfEmpty(),
                (x, b) => new EmployeeResponse(
                    x.Employee.Id,
                    x.Employee.Code,
                    x.Employee.FirstName,
                    x.Employee.LastName,
                    x.Employee.DocumentIdentifier.Type,
                    x.Employee.DocumentIdentifier.Number,
                    x.Employee.Email.HasValue ? x.Employee.Email.Value.Value : null,
                    x.Employee.Phone.HasValue ? x.Employee.Phone.Value.Value : null,
                    x.Employee.DepartmentId,
                    x.Employee.Position,
                    x.Employee.PhotoUrl,
                    x.Employee.MainBranchId,
                    b != null ? b.Name : null,
                    x.Employee.Status,
                    x.Employee.MobileCheckInEnabled,
                    x.Employee.ApplicationUserId
                )
            )
            .ToListAsync(cancellationToken);

        return employees;
    }

}


