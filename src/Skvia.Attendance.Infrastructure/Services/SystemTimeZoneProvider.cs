using Skvia.Attendance.Domain.Common;

namespace Skvia.Attendance.Infrastructure.Services;

public class SystemTimeZoneProvider : ITimeZoneProvider
{
    public TimeZoneInfo GetTimeZone(string timeZoneId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(timeZoneId);
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }
}
