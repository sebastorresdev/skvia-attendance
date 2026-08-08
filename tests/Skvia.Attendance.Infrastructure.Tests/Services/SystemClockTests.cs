using FluentAssertions;
using Skvia.Attendance.Infrastructure.Services;

namespace Skvia.Attendance.Infrastructure.Tests.Services;

public class SystemClockTests
{
    [Fact]
    public void UtcNow_WhenCalled_ShouldReturnCurrentUtcTime()
    {
        // Arrange
        var clock = new SystemClock();

        // Act
        var result = clock.UtcNow;

        // Assert
        result.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }
}
