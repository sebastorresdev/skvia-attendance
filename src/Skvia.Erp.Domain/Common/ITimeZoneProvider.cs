namespace Skvia.Erp.Domain.Common;

public interface ITimeZoneProvider
{
    TimeZoneInfo GetTimeZone(string timeZoneId);
}

