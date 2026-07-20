using Skvia.Attendance.Application.Employees.DTOs;

namespace Skvia.Attendance.Application.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler(IApplicationDbContext db) : IQueryHandler<GetEmployeesQuery, ErrorOr<List<GetEmployeeResponse>>>
{
    public async Task<ErrorOr<List<GetEmployeeResponse>>> HandleAsync(GetEmployeesQuery query, CancellationToken cancellationToken)
    {
        var employees = await db.Employees
            .AsNoTracking()
            .Select(e => new GetEmployeeResponse
            (
                Id : e.Id,
                Code : e.Code,
                FirstName : e.FirstName,
                LastName : e.LastName,
                Department : e.Department,
                PhotoUrl : e.PhotoUrl
            ))
            .ToListAsync(cancellationToken);

        return employees;
    }
}
