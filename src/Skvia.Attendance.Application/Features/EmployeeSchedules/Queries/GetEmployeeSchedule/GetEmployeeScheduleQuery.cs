using Skvia.Attendance.Application.Features.EmployeeSchedules.DTOs;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Queries.GetEmployeeSchedule;

public record GetEmployeeScheduleQuery(
    Guid EmployeeId,
    DateOnly StartDate,
    DateOnly EndDate) : IQuery<ErrorOr<List<EmployeeScheduleResponse>>>;
