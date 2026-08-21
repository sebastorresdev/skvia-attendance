using Skvia.Erp.Domain.Common;

namespace Skvia.Erp.Infrastructure.Services;

public class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

