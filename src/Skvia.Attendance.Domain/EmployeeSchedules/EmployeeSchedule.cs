using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Domain.Schedules;

namespace Skvia.Attendance.Domain.EmployeeSchedules;

public class EmployeeSchedule : BaseEntity
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    public Guid? ScheduleId { get; private set; }
    public Schedule? Schedule { get; private set; }

    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }

    // Compatibilidad y soporte diario
    public DateOnly Date { get; private set; }
    public TimeOnly? AssignedStartTime { get; private set; }
    public TimeOnly? AssignedEndTime { get; private set; }
    public Guid? BaseScheduleId { get; private set; }
    public ScheduleDayType DayType { get; private set; }

    private EmployeeSchedule() { }

    public static ErrorOr<EmployeeSchedule> CreateAssignment(
        Guid employeeId,
        Guid scheduleId,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo = null)
    {
        if (employeeId == Guid.Empty)
            return Error.Validation("EmployeeSchedule.InvalidEmployee", "El empleado es requerido.");

        if (scheduleId == Guid.Empty)
            return Error.Validation("EmployeeSchedule.InvalidSchedule", "La plantilla de horario es requerida.");

        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
            return Error.Validation("EmployeeSchedule.InvalidRange", "La fecha final de vigencia no puede ser menor a la fecha inicial.");

        return new EmployeeSchedule
        {
            EmployeeId = employeeId,
            ScheduleId = scheduleId,
            BaseScheduleId = scheduleId,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            Date = effectiveFrom,
            DayType = ScheduleDayType.WorkDay
        };
    }

    public ErrorOr<Success> UpdateRange(DateOnly effectiveFrom, DateOnly? effectiveTo)
    {
        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
            return Error.Validation("EmployeeSchedule.InvalidRange", "La fecha final de vigencia no puede ser menor a la fecha inicial.");

        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        return Result.Success;
    }

    private static ErrorOr<EmployeeSchedule> Create(
        Guid employeeId,
        DateOnly date,
        ScheduleDayType type,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        Guid? baseScheduleId = null)
    {
        if (employeeId == Guid.Empty)
            throw new DomainException("El empleado es requerido.");

        ErrorOr<Success> result = ValidateHoursForDayType(type, startTime, endTime);

        if (result.IsError)
            return result.Errors;

        return new EmployeeSchedule
        {
            EmployeeId = employeeId,
            Date = date,
            EffectiveFrom = date,
            EffectiveTo = date,
            AssignedStartTime = startTime,
            AssignedEndTime = endTime,
            BaseScheduleId = baseScheduleId,
            ScheduleId = baseScheduleId,
            DayType = type
        };
    }

    public static ErrorOr<EmployeeSchedule> CreateWorkDay(Guid employeeId, DateOnly date, TimeOnly startTime, TimeOnly endTime, Guid? baseScheduleId = null)
        => Create(employeeId, date, ScheduleDayType.WorkDay, startTime, endTime, baseScheduleId);

    public static ErrorOr<EmployeeSchedule> CreateRestDay(Guid employeeId, DateOnly date)
        => Create(employeeId, date, ScheduleDayType.DayOff);

    public static ErrorOr<EmployeeSchedule> CreateVacationDay(Guid employeeId, DateOnly date)
        => Create(employeeId, date, ScheduleDayType.Vacation);

    public static ErrorOr<EmployeeSchedule> CreateMedicalLeaveDay(Guid employeeId, DateOnly date)
        => Create(employeeId, date, ScheduleDayType.MedicalLeave);

    public static ErrorOr<EmployeeSchedule> CreateMakeUpDay(Guid employeeId, DateOnly date, TimeOnly startTime, TimeOnly endTime, Guid? baseScheduleId = null)
        => Create(employeeId, date, ScheduleDayType.MakeUpDay, startTime, endTime, baseScheduleId);

    private static ErrorOr<Success> ValidateHoursForDayType(
        ScheduleDayType type,
        TimeOnly? startTime,
        TimeOnly? endTime)
    {
        List<Error> errors = [];

        bool requiresHours = type is ScheduleDayType.WorkDay or ScheduleDayType.MakeUpDay;

        if (requiresHours)
        {
            if (startTime is null || endTime is null)
            {
                errors.Add(Error.Validation(description: "Los días laborables requieren hora de inicio y fin."));
            }
            else if (endTime <= startTime)
            {
                errors.Add(Error.Validation(description: "La hora de fin debe ser posterior a la hora de inicio."));
            }
        }
        else
        {
            if (startTime is not null || endTime is not null)
            {
                errors.Add(Error.Validation(description: "Los días no laborables no requieren hora de inicio ni fin."));
            }
        }

        return errors.Count == 0 ? Result.Success : errors;
    }
}

