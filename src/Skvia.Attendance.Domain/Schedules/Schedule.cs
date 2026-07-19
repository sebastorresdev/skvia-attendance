namespace Skvia.Attendance.Domain.Schedules;

// Horario
public class Schedule : BaseEntity
{
    public string Name { get; private set => field = value.Trim(); } = null!;

    public TimeOnly DefaultStartTime { get; private set; }
    public TimeOnly DefaultEndTime { get; private set; }

    // Constructor privado para EF Core
    private Schedule() { }

    public static Schedule Create(string name, TimeOnly startTime, TimeOnly endTime)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return new Schedule
        {
            Name = name,
            DefaultStartTime = startTime,
            DefaultEndTime = endTime
        };
    }

    // Método para cuando RRHH necesite ajustar las horas base del turno
    public void UpdateTimes(string name, TimeOnly startTime, TimeOnly endTime)
    {
        Name = name;
        DefaultStartTime = startTime;
        DefaultEndTime = endTime;
    }
}
