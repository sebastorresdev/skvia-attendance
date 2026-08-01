namespace Skvia.Attendance.Domain.Common;

public interface ITimeZoneProvider
{
    TimeZoneInfo GetTimeZone(string timeZoneId);
}
