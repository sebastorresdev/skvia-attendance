using Skvia.Erp.Domain.Common;

namespace Skvia.Erp.Infrastructure.Services;

public class SystemTimeZoneProvider : ITimeZoneProvider
{
    public TimeZoneInfo GetTimeZone(string timeZoneId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(timeZoneId);
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }
}

