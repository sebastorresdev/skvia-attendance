using ErrorOr;
using FluentAssertions;
using Moq;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Roles.Commands.UpdateRole;

namespace Skvia.Attendance.Application.Tests.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandlerTests
{
    private readonly Mock<IRoleService> _roleServiceMock;
    private readonly UpdateRoleCommandHandler _handler;

    public UpdateRoleCommandHandlerTests()
    {
        _roleServiceMock = new Mock<IRoleService>();
        _handler = new UpdateRoleCommandHandler(_roleServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WhenUpdateIsSuccessful_ReturnsSuccess()
    {
        // Arrange
        var command = new UpdateRoleCommand(Guid.NewGuid(), "Admin", "Administrador del sistema");
        var successResult = Result.Success;
        
        _roleServiceMock
            .Setup(x => x.UpdateRoleAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        
        _roleServiceMock.Verify(x => x.UpdateRoleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenUpdateFails_ReturnsError()
    {
        // Arrange
        var command = new UpdateRoleCommand(Guid.NewGuid(), "Admin", "Administrador del sistema");
        var errorResult = Error.NotFound(description: "Role not found");
        
        _roleServiceMock
            .Setup(x => x.UpdateRoleAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(errorResult);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
        result.FirstError.Description.Should().Be("Role not found");
        
        _roleServiceMock.Verify(x => x.UpdateRoleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }
}
