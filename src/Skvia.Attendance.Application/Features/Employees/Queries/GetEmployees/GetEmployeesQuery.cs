using Skvia.Attendance.Application.Features.Employees.DTOs;

namespace Skvia.Attendance.Application.Features.Employees.Queries.GetEmployees;

public record GetEmployeesQuery() : IQuery<ErrorOr<List<EmployeeResponse>>>;
