using Skvia.Attendance.Application.Features.Employees.DTOs;

namespace Skvia.Attendance.Application.Features.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(Guid EmployeeId) : IQuery<ErrorOr<EmployeeDetailResponse>>;
