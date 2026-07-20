using Skvia.Attendance.Application.Employees.DTOs;

namespace Skvia.Attendance.Application.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(Guid EmployeeId) : IQuery<ErrorOr<GetEmployeeByIdResponse>>;
