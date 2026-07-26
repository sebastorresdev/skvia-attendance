using Skvia.Attendance.Application.Employees.DTOs;

namespace Skvia.Attendance.Application.Employees.Queries.GetEmployees;

public record GetEmployeesQuery() : IQuery<ErrorOr<List<EmployeeResponse>>>;
