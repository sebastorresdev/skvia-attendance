using ErrorOr;
using FluentAssertions;
using NSubstitute;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Roles.Commands.UpdateRole;

namespace Skvia.Attendance.Application.Tests.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandlerTests
{
    private readonly IRoleService _roleServiceMock;
    private readonly UpdateRoleCommandHandler _handler;

    public UpdateRoleCommandHandlerTests()
    {
        _roleServiceMock = Substitute.For<IRoleService>();
        _handler = new UpdateRoleCommandHandler(_roleServiceMock);
    }

    [Fact]
    public async Task HandleAsync_WhenUpdateIsSuccessful_ShouldReturnSuccess()
    {
        // Arrange
        var command = new UpdateRoleCommand(Guid.NewGuid(), "Admin", "Administrador del sistema");
        var successResult = Result.Success;

        _roleServiceMock
            .UpdateRoleAsync(command, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ErrorOr<Success>>(successResult));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();

        await _roleServiceMock
            .Received(1)
            .UpdateRoleAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenUpdateFails_ShouldReturnError()
    {
        // Arrange
        var command = new UpdateRoleCommand(Guid.NewGuid(), "Admin", "Administrador del sistema");
        ErrorOr<Success> errorResult = Error.NotFound(description: "Role not found");

        _roleServiceMock
            .UpdateRoleAsync(command, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ErrorOr<Success>>(errorResult));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
        result.FirstError.Description.Should().Be("Role not found");

        await _roleServiceMock
            .Received(1)
            .UpdateRoleAsync(command, Arg.Any<CancellationToken>());
    }
}
