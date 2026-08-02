using Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.AssignWeeklySchedule;

namespace Skvia.Attendance.Api.Endpoints.Employees.Requests;

public class AssignWeeklyScheduleRequest
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<DailyScheduleRequest> Days { get; set; } = [];
}
