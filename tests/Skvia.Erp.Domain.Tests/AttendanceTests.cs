using FluentAssertions;
using Skvia.Erp.Domain.Common;
using AttendanceEntity = Skvia.Erp.Domain.Attendances.Attendance;

namespace Skvia.Erp.Domain.Tests;

public class AttendanceTests
{
    private class TestClock(DateTimeOffset utcNow) : IClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }

    private class TestTimeZoneProvider : ITimeZoneProvider
    {
        public TimeZoneInfo GetTimeZone(string timeZoneId) => TimeZoneInfo.Utc;
    }

    [Fact]
    public void CreateCheckIn_WhenOnTime_ShouldSetIsLateToFalse()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var workplaceId = Guid.NewGuid();
        var photoUrl = "https://example.com/checkin.jpg";
        var scheduledStart = new TimeOnly(8, 0);
        var clockTime = new DateTimeOffset(2026, 8, 8, 8, 0, 0, TimeSpan.Zero);
        var clock = new TestClock(clockTime);
        var tzProvider = new TestTimeZoneProvider();

        // Act
        var attendance = AttendanceEntity.CreateCheckIn(
            employeeId,
            workplaceId,
            photoUrl,
            isValidCheckIn: true,
            scheduledStartTime: scheduledStart,
            operationTimeZoneId: "UTC",
            clock: clock,
            timeZoneProvider: tzProvider,
            tardinessToleranceMinutes: 5);

        // Assert
        attendance.Should().NotBeNull();
        attendance.EmployeeId.Should().Be(employeeId);
        attendance.CheckInWorkplaceId.Should().Be(workplaceId);
        attendance.PhotoCheckIn.Should().Be(photoUrl);
        attendance.IsLate.Should().BeFalse();
        attendance.MinutesLate.Should().Be(0);
        attendance.Date.Should().Be(new DateOnly(2026, 8, 8));
    }

    [Fact]
    public void CreateCheckIn_WhenLateBeyondTolerance_ShouldSetIsLateToTrue()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var workplaceId = Guid.NewGuid();
        var photoUrl = "https://example.com/checkin.jpg";
        var scheduledStart = new TimeOnly(8, 0);
        var clockTime = new DateTimeOffset(2026, 8, 8, 8, 20, 0, TimeSpan.Zero); // 20 min late
        var clock = new TestClock(clockTime);
        var tzProvider = new TestTimeZoneProvider();

        // Act
        var attendance = AttendanceEntity.CreateCheckIn(
            employeeId,
            workplaceId,
            photoUrl,
            isValidCheckIn: true,
            scheduledStartTime: scheduledStart,
            operationTimeZoneId: "UTC",
            clock: clock,
            timeZoneProvider: tzProvider,
            tardinessToleranceMinutes: 10);

        // Assert
        attendance.IsLate.Should().BeTrue();
        attendance.MinutesLate.Should().Be(20);
    }

    [Fact]
    public void StartBreak_WhenValid_ShouldSetBreakStart()
    {
        // Arrange
        var clockTime = new DateTimeOffset(2026, 8, 8, 8, 0, 0, TimeSpan.Zero);
        var clock = new TestClock(clockTime);
        var attendance = AttendanceEntity.CreateCheckIn(
            Guid.NewGuid(), Guid.NewGuid(), "checkin.jpg", true, new TimeOnly(8, 0), "UTC", clock, new TestTimeZoneProvider());

        var breakStartClock = new TestClock(clockTime.AddHours(4));

        // Act
        attendance.StartBreak("break_start.jpg", breakStartClock);

        // Assert
        attendance.BreakStart.Should().Be(clockTime.AddHours(4));
        attendance.PhotoBreakStart.Should().Be("break_start.jpg");
    }

    [Fact]
    public void StartBreak_WhenAlreadyStarted_ShouldThrowDomainException()
    {
        // Arrange
        var clockTime = new DateTimeOffset(2026, 8, 8, 8, 0, 0, TimeSpan.Zero);
        var clock = new TestClock(clockTime);
        var attendance = AttendanceEntity.CreateCheckIn(
            Guid.NewGuid(), Guid.NewGuid(), "checkin.jpg", true, new TimeOnly(8, 0), "UTC", clock, new TestTimeZoneProvider());

        attendance.StartBreak("break1.jpg", clock);

        // Act
        Action act = () => attendance.StartBreak("break2.jpg", clock);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("El refrigerio ya fue iniciado.");
    }

    [Fact]
    public void EndBreak_WhenBreakNotStarted_ShouldThrowDomainException()
    {
        // Arrange
        var clock = new TestClock(DateTimeOffset.UtcNow);
        var attendance = AttendanceEntity.CreateCheckIn(
            Guid.NewGuid(), Guid.NewGuid(), "checkin.jpg", true, new TimeOnly(8, 0), "UTC", clock, new TestTimeZoneProvider());

        // Act
        Action act = () => attendance.EndBreak("break_end.jpg", clock);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("No se puede finalizar un refrigerio no iniciado.");
    }

    [Fact]
    public void RegisterCheckOut_WhenValid_ShouldCalculateMinutesWorkedAndOvertime()
    {
        // Arrange
        var startTime = new DateTimeOffset(2026, 8, 8, 8, 0, 0, TimeSpan.Zero);
        var clockStart = new TestClock(startTime);
        var tz = new TestTimeZoneProvider();

        var attendance = AttendanceEntity.CreateCheckIn(
            Guid.NewGuid(), Guid.NewGuid(), "checkin.jpg", true, new TimeOnly(8, 0), "UTC", clockStart, tz);

        // Break 12:00 to 13:00 (60 mins)
        var breakStartClock = new TestClock(startTime.AddHours(4));
        var breakEndClock = new TestClock(startTime.AddHours(5));
        attendance.StartBreak("break_start.jpg", breakStartClock);
        attendance.EndBreak("break_end.jpg", breakEndClock);

        // Checkout at 18:00 (10 hours total - 1 hour break = 9 hours / 540 mins worked)
        var checkOutClock = new TestClock(startTime.AddHours(10));

        // Act (scheduled 8 hours = 480 mins)
        attendance.RegisterCheckOut(Guid.NewGuid(), "checkout.jpg", true, totalMinutesScheduled: 480, checkOutClock);

        // Assert
        attendance.CheckOut.Should().Be(startTime.AddHours(10));
        attendance.MinutesWorked.Should().Be(540);
        attendance.OvertimeMinutes.Should().Be(60);
    }
}

