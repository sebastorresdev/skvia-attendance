using Skvia.Attendance.Domain.Common;

namespace Skvia.Attendance.Infrastructure.Services;

public class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
