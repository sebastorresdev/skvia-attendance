using Skvia.Attendance.Application.Features.Departments.DTOs;

using ErrorOr;

namespace Skvia.Attendance.Application.Features.Departments.Queries.GetDepartments;

public record GetDepartmentsQuery() : IQuery<ErrorOr<List<DepartmentResponse>>>;
