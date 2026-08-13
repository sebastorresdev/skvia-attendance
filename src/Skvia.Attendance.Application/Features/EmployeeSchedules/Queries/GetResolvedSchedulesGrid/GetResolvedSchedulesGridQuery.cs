using Skvia.Attendance.Application.Features.EmployeeSchedules.DTOs;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Queries.GetResolvedSchedulesGrid;

public record EmployeeScheduleGridRowDto(
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeCode,
    string? DepartmentName,
    string? BranchName,
    List<ResolvedScheduleDayDto> Days);

public record GetResolvedSchedulesGridQuery(
    Guid? BranchId,
    Guid? DepartmentId,
    DateOnly StartDate,
    DateOnly EndDate) : IQuery<ErrorOr<List<EmployeeScheduleGridRowDto>>>;
