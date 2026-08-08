using FluentAssertions;
using Skvia.Attendance.Infrastructure.Services;

namespace Skvia.Attendance.Infrastructure.Tests.Services;

public class SystemTimeZoneProviderTests
{
    private readonly SystemTimeZoneProvider _provider = new();

    [Theory]
    [InlineData("UTC")]
    [InlineData("SA Pacific Standard Time")]
    public void GetTimeZone_WhenValidTimeZoneId_ShouldReturnTimeZoneInfo(string timeZoneId)
    {
        // Act
        var result = _provider.GetTimeZone(timeZoneId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(timeZoneId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetTimeZone_WhenNullOrWhiteSpaceId_ShouldThrowArgumentException(string? invalidTimeZoneId)
    {
        // Act
        Action act = () => _provider.GetTimeZone(invalidTimeZoneId!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
