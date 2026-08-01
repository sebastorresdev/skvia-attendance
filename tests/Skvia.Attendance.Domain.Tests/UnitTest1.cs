using Skvia.Attendance.Domain.Attendances;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Domain.Tests;

public class AttendanceDomainTests
{
    [Fact]
    public void CreateCheckIn_UsesProvidedClockAndTimeZone()
    {
        var clock = new TestClock(new DateTimeOffset(2024, 6, 10, 14, 15, 0, TimeSpan.Zero));
        var timeZoneProvider = new TestTimeZoneProvider();

        var attendance = Skvia.Attendance.Domain.Attendances.Attendance.CreateCheckIn(
            employeeId: Guid.NewGuid(),
            branchId: Guid.NewGuid(),
            photoUrl: "photo.png",
            isValidCheckIn: true,
            scheduledStartTime: new TimeOnly(9, 0),
            operationTimeZoneId: "America/Lima",
            clock,
            timeZoneProvider);

        var expectedLocalDateTime = TimeZoneInfo.ConvertTime(clock.UtcNow, timeZoneProvider.GetTimeZone("America/Lima")).DateTime;

        Assert.Equal(15, attendance.MinutesLate);
        Assert.True(attendance.IsLate);
        Assert.Equal(DateOnly.FromDateTime(expectedLocalDateTime), attendance.Date);
    }

    [Fact]
    public void CreateWorkDay_RequiresHoursForWorkingDays()
    {
        var result = EmployeeSchedule.CreateWorkDay(Guid.NewGuid(), new DateOnly(2024, 6, 10), Guid.NewGuid(), new TimeOnly(8, 0), new TimeOnly(18, 0));

        Assert.True(result.IsError is false);
        Assert.True(result.Value is not null);
    }

    private sealed class TestClock(DateTimeOffset utcNow) : IClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }

    private sealed class TestTimeZoneProvider : ITimeZoneProvider
    {
        public TimeZoneInfo GetTimeZone(string timeZoneId)
            => TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }
}
