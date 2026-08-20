using ErrorOr;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Skvia.Attendance.Application.Common.Messaging;
using Skvia.Attendance.Application.Features.Attendances.Commands.CheckIn;
using Skvia.Attendance.Application.Features.Attendances.Commands.CheckOut;
using Skvia.Attendance.Application.Features.Attendances.Commands.EndBreak;
using Skvia.Attendance.Application.Features.Attendances.Commands.MobileClock;
using Skvia.Attendance.Application.Features.Attendances.Commands.StartBreak;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Domain.Workplaces;
using Skvia.Attendance.Infrastructure.Data;
using Xunit;

namespace Skvia.Attendance.Application.Tests.Features.Attendances;

public class MobileClockCommandHandlerTests
{
    private static ApplicationDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task HandleAsync_WhenLocationIsMissing_ShouldReturnLocationRequiredError()
    {
        // Arrange
        await using var dbContext = CreateInMemoryDbContext();
        var employee = Employee.Create("EMP001", "Juan", "Pérez", DocumentIdentifier.Create(DocumentType.Dni, "12345678"), DateTimeOffset.UtcNow);
        var workplace = Workplace.Create("W001", "Sede Central", "America/Lima", -12.046374, -77.042793, 100);
        employee.SetAllowedWorkplaceIds([workplace.Id]);
        dbContext.Employees.Add(employee);
        dbContext.Workplaces.Add(workplace);
        await dbContext.SaveChangesAsync();

        var checkInHandler = Substitute.For<ICommandHandler<CheckInCommand, ErrorOr<Success>>>();
        var startBreakHandler = Substitute.For<ICommandHandler<StartBreakCommand, ErrorOr<Success>>>();
        var endBreakHandler = Substitute.For<ICommandHandler<EndBreakCommand, ErrorOr<Success>>>();
        var checkOutHandler = Substitute.For<ICommandHandler<CheckOutCommand, ErrorOr<Success>>>();

        var handler = new MobileClockCommandHandler(dbContext, checkInHandler, startBreakHandler, endBreakHandler, checkOutHandler);
        var command = new MobileClockCommand("USER001", "EMP001", "ENTRADA", Latitud: null, Longitud: null, PhotoUrl: null);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("MobileClock.LocationRequired");
    }

    [Fact]
    public async Task HandleAsync_WhenEmployeeIsOutOfGeofence_ShouldReturnOutOfRangeError()
    {
        // Arrange
        await using var dbContext = CreateInMemoryDbContext();
        var employee = Employee.Create("EMP001", "Juan", "Pérez", DocumentIdentifier.Create(DocumentType.Dni, "12345678"), DateTimeOffset.UtcNow);
        var workplace = Workplace.Create("W001", "Sede Central", "America/Lima", -12.046374, -77.042793, 100);
        employee.SetAllowedWorkplaceIds([workplace.Id]);
        dbContext.Employees.Add(employee);
        dbContext.Workplaces.Add(workplace);
        await dbContext.SaveChangesAsync();

        var checkInHandler = Substitute.For<ICommandHandler<CheckInCommand, ErrorOr<Success>>>();
        var startBreakHandler = Substitute.For<ICommandHandler<StartBreakCommand, ErrorOr<Success>>>();
        var endBreakHandler = Substitute.For<ICommandHandler<EndBreakCommand, ErrorOr<Success>>>();
        var checkOutHandler = Substitute.For<ICommandHandler<CheckOutCommand, ErrorOr<Success>>>();

        var handler = new MobileClockCommandHandler(dbContext, checkInHandler, startBreakHandler, endBreakHandler, checkOutHandler);
        // Coordinates ~5 km away
        var command = new MobileClockCommand("USER001", "EMP001", "ENTRADA", Latitud: -12.090000, Longitud: -77.030000, PhotoUrl: null);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("MobileClock.OutOfRange");
    }

    [Fact]
    public async Task HandleAsync_WhenEmployeeIsWithinGeofence_ShouldRegisterAttendanceSuccessfully()
    {
        // Arrange
        await using var dbContext = CreateInMemoryDbContext();
        var employee = Employee.Create("EMP001", "Juan", "Pérez", DocumentIdentifier.Create(DocumentType.Dni, "12345678"), DateTimeOffset.UtcNow);
        var workplace = Workplace.Create("W001", "Sede Central", "America/Lima", -12.046374, -77.042793, 200);
        employee.SetAllowedWorkplaceIds([workplace.Id]);
        dbContext.Employees.Add(employee);
        dbContext.Workplaces.Add(workplace);
        await dbContext.SaveChangesAsync();

        var checkInHandler = Substitute.For<ICommandHandler<CheckInCommand, ErrorOr<Success>>>();
        checkInHandler.HandleAsync(Arg.Any<CheckInCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success);

        var startBreakHandler = Substitute.For<ICommandHandler<StartBreakCommand, ErrorOr<Success>>>();
        var endBreakHandler = Substitute.For<ICommandHandler<EndBreakCommand, ErrorOr<Success>>>();
        var checkOutHandler = Substitute.For<ICommandHandler<CheckOutCommand, ErrorOr<Success>>>();

        var handler = new MobileClockCommandHandler(dbContext, checkInHandler, startBreakHandler, endBreakHandler, checkOutHandler);
        // Coordinates ~20m away from workplace
        var command = new MobileClockCommand("USER001", "EMP001", "ENTRADA", Latitud: -12.046350, Longitud: -77.042790, PhotoUrl: "photo.jpg");

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Success.Should().BeTrue();
        result.Value.TipoMarcacion.Should().Be("ENTRADA");
    }
}
