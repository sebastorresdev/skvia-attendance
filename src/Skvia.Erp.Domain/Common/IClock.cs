namespace Skvia.Erp.Domain.Common;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

