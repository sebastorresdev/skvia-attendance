using Skvia.Erp.Domain.Common;
using ErrorOr;
namespace Skvia.Erp.Domain.Schedules;

// Horario
public class Schedule : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string TimeZoneId { get; private set; } = null!;
    public bool HasBreak { get; private set; }
    public TimeOnly? BreakStartTime { get; private set; }
    public TimeOnly? BreakEndTime { get; private set; }

    public TimeOnly DefaultStartTime { get; private set; }
    public TimeOnly DefaultEndTime { get; private set; }

    // Constructor privado para EF Core
    private Schedule() { }

    public static ErrorOr<Schedule> Create(
        string code,
        string description,
        string timeZoneId,
        TimeOnly startTime,
        TimeOnly endTime,
        bool hasBreak = false,
        TimeOnly? breakStartTime = null,
        TimeOnly? breakEndTime = null)
    {
        if (string.IsNullOrWhiteSpace(code)) return Error.Validation(description: "El código es requerido.");
        if (string.IsNullOrWhiteSpace(description)) return Error.Validation(description: "La descripción es requerida.");
        if (string.IsNullOrWhiteSpace(timeZoneId)) return Error.Validation(description: "La zona horaria es requerida.");
        
        if (endTime <= startTime)
            return Error.Validation(description: "La hora de fin debe ser posterior a la de inicio.");
            
        if (hasBreak)
        {
            if (breakStartTime is null || breakEndTime is null)
                return Error.Validation(description: "Debe especificar el inicio y fin del break.");
            if (breakEndTime <= breakStartTime)
                return Error.Validation(description: "La hora de fin de break debe ser posterior a la de inicio.");
        }

        return new Schedule
        {
            Code = code.Trim().ToUpper(),
            Description = description.Trim(),
            TimeZoneId = timeZoneId.Trim(),
            DefaultStartTime = startTime,
            DefaultEndTime = endTime,
            HasBreak = hasBreak,
            BreakStartTime = hasBreak ? breakStartTime : null,
            BreakEndTime = hasBreak ? breakEndTime : null
        };
    }


    public ErrorOr<Success> UpdateTimes(
        string code,
        string description,
        string timeZoneId,
        TimeOnly startTime,
        TimeOnly endTime,
        bool hasBreak = false,
        TimeOnly? breakStartTime = null,
        TimeOnly? breakEndTime = null)
    {
        if (string.IsNullOrWhiteSpace(code)) return Error.Validation(description: "El código es requerido.");
        if (string.IsNullOrWhiteSpace(description)) return Error.Validation(description: "La descripción es requerida.");
        if (string.IsNullOrWhiteSpace(timeZoneId)) return Error.Validation(description: "La zona horaria es requerida.");

        if (endTime <= startTime)
            return Error.Validation(description: "La hora de fin debe ser posterior a la de inicio.");
            
        if (hasBreak)
        {
            if (breakStartTime is null || breakEndTime is null)
                return Error.Validation(description: "Debe especificar el inicio y fin del break.");
            if (breakEndTime <= breakStartTime)
                return Error.Validation(description: "La hora de fin de break debe ser posterior a la de inicio.");
        }

        Code = code.Trim().ToUpper();
        Description = description.Trim();
        TimeZoneId = timeZoneId.Trim();
        DefaultStartTime = startTime;
        DefaultEndTime = endTime;
        HasBreak = hasBreak;
        BreakStartTime = hasBreak ? breakStartTime : null;
        BreakEndTime = hasBreak ? breakEndTime : null;
        
        return Result.Success;
    }
}


