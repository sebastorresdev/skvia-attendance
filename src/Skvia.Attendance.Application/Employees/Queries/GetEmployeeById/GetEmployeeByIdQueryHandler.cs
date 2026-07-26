using Skvia.Attendance.Application.Employees.DTOs;
using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler(IApplicationDbContext db) : IQueryHandler<GetEmployeeByIdQuery, ErrorOr<EmployeeDetailResponse>>
{
    public async Task<ErrorOr<EmployeeDetailResponse>> HandleAsync(GetEmployeeByIdQuery query, CancellationToken cancellationToken)
    {
        var employee = await db.Employees
            .AsNoTracking()
            .Where(e => e.Id == query.EmployeeId)
            .Select(e => new EmployeeDetailResponse(
                e.Id,
                e.Code,
                e.FirstName,
                e.LastName,
                e.DocumentType,
                e.DocumentNumber,
                e.Email,
                e.Phone,
                e.Position,
                e.Department,
                e.HireDate,
                e.PhotoUrl))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (employee is null)
        {
            return EmployeeErrors.NotFound;
        }

        return employee;
    }
}
