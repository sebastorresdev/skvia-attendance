using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Domain.EmployeeSchedules;

public class EmployeeSchedulePattern : BaseEntity
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public DayOfWeek DayOfWeek { get; private set; }
    public bool IsWorkDay { get; private set; }
    public TimeOnly? StartTime { get; private set; }
    public TimeOnly? EndTime { get; private set; }

    private EmployeeSchedulePattern() { }

    public static EmployeeSchedulePattern Create(Guid employeeId, DayOfWeek dayOfWeek, bool isWorkDay, TimeOnly? startTime, TimeOnly? endTime)
    {
        return new EmployeeSchedulePattern
        {
            EmployeeId = employeeId,
            DayOfWeek = dayOfWeek,
            IsWorkDay = isWorkDay,
            StartTime = startTime,
            EndTime = endTime
        };
    }

    public void Update(bool isWorkDay, TimeOnly? startTime, TimeOnly? endTime)
    {
        IsWorkDay = isWorkDay;
        StartTime = startTime;
        EndTime = endTime;
    }
}
