using Skvia.Attendance.Domain.Branches;
using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Domain.Attendances;

public class Attendance : BaseEntity
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    public DateOnly Date { get; private set; }

    public DateTimeOffset CheckIn { get; private set; }
    public string PhotoCheckIn { get; private set; } = null!;
    public Guid CheckInBranchId { get; private set; }
    public Branch CheckInBranch { get; private set; } = null!;

    public DateTimeOffset? BreakStart { get; private set; }
    public string? PhotoBreakStart { get; private set; }
    public DateTimeOffset? BreakEnd { get; private set; }
    public string? PhotoBreakEnd { get; private set; }

    public DateTimeOffset? CheckOut { get; private set; }
    public string? PhotoCheckOut { get; private set; }
    public Guid? CheckOutBranchId { get; private set; }
    public Branch? CheckOutBranch { get; private set; }

    public bool IsLate { get; private set; }
    public int MinutesLate { get; private set; }
    public bool IsValidCheckIn { get; private set; }
    public bool IsValidCheckOut { get; private set; }
    public int MinutesWorked { get; private set; }
    public int OvertimeMinutes { get; private set; }
    public AttendanceSource Source { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public string? DeviceId { get; private set; }

    private Attendance() { }

    public static Attendance CreateCheckIn(
        Guid employeeId,
        Guid branchId,
        string photoUrl,
        bool isValidCheckIn,
        TimeOnly scheduledStartTime,
        string operationTimeZoneId,
        IClock clock,
        ITimeZoneProvider timeZoneProvider,
        AttendanceSource source = AttendanceSource.Kiosk,
        double? latitude = null,
        double? longitude = null,
        string? deviceId = null,
        int branchTardinessToleranceMinutes = 0)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(employeeId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(branchId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(photoUrl);

        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(timeZoneProvider);

        var utcNow = clock.UtcNow;
        var branchTimeZone = timeZoneProvider.GetTimeZone(operationTimeZoneId);
        var localTime = TimeZoneInfo.ConvertTime(utcNow, branchTimeZone);

        var currentTime = TimeOnly.FromDateTime(localTime.DateTime);
        var currentDate = DateOnly.FromDateTime(localTime.DateTime);

        var difference = currentTime > scheduledStartTime
            ? (int)(currentTime - scheduledStartTime).TotalMinutes
            : 0;
            
        var minutesLate = difference > branchTardinessToleranceMinutes ? difference : 0;

        return new Attendance
        {
            EmployeeId = employeeId,
            Date = currentDate,
            CheckIn = utcNow,
            CheckInBranchId = branchId,
            PhotoCheckIn = photoUrl.Trim(),
            IsValidCheckIn = isValidCheckIn,
            MinutesLate = minutesLate,
            IsLate = minutesLate > 0,
            Source = source,
            Latitude = latitude,
            Longitude = longitude,
            DeviceId = deviceId?.Trim()
        };
    }

    public void StartBreak(string photoUrl, IClock clock)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentException.ThrowIfNullOrWhiteSpace(photoUrl, "La foto es requerida.");
        if (CheckOut.HasValue) throw new DomainException("No se puede iniciar break después del check-out.");
        if (BreakStart.HasValue) throw new DomainException("El refrigerio ya fue iniciado.");

        BreakStart = clock.UtcNow;
        PhotoBreakStart = photoUrl.Trim();
    }

    public void EndBreak(string photoUrl, IClock clock)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentException.ThrowIfNullOrWhiteSpace(photoUrl, "La foto es requerida.");
        if (!BreakStart.HasValue) throw new DomainException("No se puede finalizar un refrigerio no iniciado.");
        if (BreakEnd.HasValue) throw new DomainException("El refrigerio ya fue finalizado.");

        BreakEnd = clock.UtcNow;
        PhotoBreakEnd = photoUrl.Trim();
    }

    public void RegisterCheckOut(Guid branchId, string photoUrl, bool isValidCheckOut, int totalMinutesScheduled, IClock clock)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentOutOfRangeException.ThrowIfEqual(branchId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(photoUrl);
        if (CheckOut.HasValue) throw new DomainException("La asistencia ya tiene un check-out.");
        if (BreakStart.HasValue && !BreakEnd.HasValue) throw new DomainException("Termina el break primero.");

        CheckOut = clock.UtcNow;
        CheckOutBranchId = branchId;
        PhotoCheckOut = photoUrl.Trim();
        IsValidCheckOut = isValidCheckOut;

        int totalMinutes = (int)(CheckOut.Value - CheckIn).TotalMinutes;

        if (BreakStart.HasValue && BreakEnd.HasValue)
        {
            int breakMinutes = (int)(BreakEnd.Value - BreakStart.Value).TotalMinutes;
            if (breakMinutes > 0) totalMinutes -= breakMinutes;
        }

        MinutesWorked = Math.Max(totalMinutes, 0);

        if (MinutesWorked > totalMinutesScheduled)
            OvertimeMinutes = MinutesWorked - totalMinutesScheduled;
    }
}
