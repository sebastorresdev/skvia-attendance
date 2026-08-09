using ErrorOr;

namespace Skvia.Attendance.Application.Features.Dashboard.Queries.GetScheduleAlerts;

public record ScheduleAlertDto(Guid EmployeeId, string EmployeeCode, string EmployeeName, DateOnly? LastScheduleDate);

public record GetScheduleAlertsQuery : IQuery<ErrorOr<List<ScheduleAlertDto>>>;
