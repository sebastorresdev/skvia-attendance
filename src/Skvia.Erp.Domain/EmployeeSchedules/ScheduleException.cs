using Skvia.Erp.Domain.Common;
using ErrorOr;
using Skvia.Erp.Domain.Employees;
using Skvia.Erp.Domain.Schedules;

namespace Skvia.Erp.Domain.EmployeeSchedules;

public class ScheduleException : BaseEntity
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    public DateOnly Date { get; private set; }

    public Guid? CustomScheduleId { get; private set; }
    public Schedule? CustomSchedule { get; private set; }

    public bool IsDayOff { get; private set; }
    public ScheduleDayType DayType { get; private set; }

    public TimeOnly? StartTime { get; private set; }
    public TimeOnly? EndTime { get; private set; }

    public string? Reason { get; private set; }

    private ScheduleException() { }

    public static ErrorOr<ScheduleException> Create(
        Guid employeeId,
        DateOnly date,
        ScheduleDayType dayType,
        Guid? customScheduleId = null,
        bool isDayOff = false,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        string? reason = null)
    {
        if (employeeId == Guid.Empty)
            return Error.Validation("ScheduleException.InvalidEmployee", "El empleado es requerido.");

        bool isWorkDayType = dayType is ScheduleDayType.WorkDay or ScheduleDayType.MakeUpDay;

        if (isWorkDayType)
        {
            if (customScheduleId is null && (startTime is null || endTime is null))
            {
                return Error.Validation("ScheduleException.InvalidTimes", "Debe especificar un turno o las horas de inicio y fin para un día laborable.");
            }

            if (startTime.HasValue && endTime.HasValue && endTime <= startTime)
            {
                return Error.Validation("ScheduleException.InvalidTimeRange", "La hora de fin debe ser posterior a la de inicio.");
            }
        }
        else
        {
            customScheduleId = null;
            startTime = null;
            endTime = null;
        }

        return new ScheduleException
        {
            EmployeeId = employeeId,
            Date = date,
            DayType = dayType,
            CustomScheduleId = customScheduleId,
            IsDayOff = isDayOff || dayType == ScheduleDayType.DayOff || dayType == ScheduleDayType.Vacation || dayType == ScheduleDayType.MedicalLeave,
            StartTime = startTime,
            EndTime = endTime,
            Reason = reason?.Trim()
        };
    }

    public ErrorOr<Success> Update(
        ScheduleDayType dayType,
        Guid? customScheduleId = null,
        bool isDayOff = false,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        string? reason = null)
    {
        bool isWorkDayType = dayType is ScheduleDayType.WorkDay or ScheduleDayType.MakeUpDay;

        if (isWorkDayType)
        {
            if (customScheduleId is null && (startTime is null || endTime is null))
            {
                return Error.Validation("ScheduleException.InvalidTimes", "Debe especificar un turno o las horas de inicio y fin para un día laborable.");
            }

            if (startTime.HasValue && endTime.HasValue && endTime <= startTime)
            {
                return Error.Validation("ScheduleException.InvalidTimeRange", "La hora de fin debe ser posterior a la de inicio.");
            }
        }
        else
        {
            customScheduleId = null;
            startTime = null;
            endTime = null;
        }

        DayType = dayType;
        CustomScheduleId = customScheduleId;
        IsDayOff = isDayOff || dayType == ScheduleDayType.DayOff || dayType == ScheduleDayType.Vacation || dayType == ScheduleDayType.MedicalLeave;
        StartTime = startTime;
        EndTime = endTime;
        Reason = reason?.Trim();

        return Result.Success;
    }
}


