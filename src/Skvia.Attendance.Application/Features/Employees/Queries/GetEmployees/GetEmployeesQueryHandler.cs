using Skvia.Attendance.Application.Features.Employees.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Skvia.Attendance.Application.Features.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler(IApplicationDbContext db) : IQueryHandler<GetEmployeesQuery, ErrorOr<List<EmployeeResponse>>>
{
    public async Task<ErrorOr<List<EmployeeResponse>>> HandleAsync(GetEmployeesQuery query, CancellationToken cancellationToken)
    {
        var employees = await db.Employees
            .AsNoTracking()
            .Select(e => new EmployeeResponse
            (
                EmployeeId: e.Id,
                Code: e.Code,
                FirstName: e.FirstName,
                LastName: e.LastName,
                DocumentType: e.DocumentIdentifier.Type,
                DocumentNumber: e.DocumentIdentifier.Number,
                Email: e.Email != null ? e.Email.Value.Value : null,
                Phone: e.Phone != null ? e.Phone.Value.Value : null,
                Department: e.Department,
                PhotoUrl: e.PhotoUrl
            ))
            .ToListAsync(cancellationToken);

        return employees;
    }
}
