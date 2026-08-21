using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Features.Employees.DTOs;
using Skvia.Erp.Domain.Employees;

namespace Skvia.Erp.Application.Features.Employees.Queries.GetEmployeeById;

/// <summary>
/// Manejador para la consulta de detalle de un empleado por ID.
/// </summary>
public class GetEmployeeByIdQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetEmployeeByIdQuery, ErrorOr<EmployeeDetailResponse>>
{
    public async Task<ErrorOr<EmployeeDetailResponse>> HandleAsync(GetEmployeeByIdQuery query, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees.AsNoTracking()
            .Where(e => e.Id == query.EmployeeId)
            .GroupJoin(
                dbContext.Branches.AsNoTracking(),
                e => e.MainBranchId,
                b => b.Id,
                (e, branchGroup) => new { Employee = e, BranchGroup = branchGroup }
            )
            .SelectMany(
                x => x.BranchGroup.DefaultIfEmpty(),
                (x, b) => new EmployeeDetailResponse(
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
                    x.Employee.HireDate,
                    x.Employee.PhotoUrl,
                    x.Employee.MainBranchId,
                    b != null ? b.Name : null,
                    x.Employee.Status,
                    x.Employee.MobileCheckInEnabled,
                    x.Employee.ApplicationUserId,
                    x.Employee.RequireFourPointAttendance,
                    x.Employee.IsAttendanceTracked,
                    x.Employee.AutoCompleteClockOut,
                    x.Employee.TardinessToleranceMinutes,
                    x.Employee.AllowedWorkplaceIds)
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound;
        }

        return employee;
    }
}


