using FluentValidation.TestHelper;
using Skvia.Erp.Application.Features.Users.Commands.CreateUser;

namespace Skvia.Erp.Application.Tests.Features.Users;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName: "johndoe",
            Password: "Password123!",
            Email: "john@example.com",
            DisplayName: "John Doe",
            PhoneNumber: null,
            PhotoUrl: null,
            BranchIds: [Guid.NewGuid()],
            RoleIds: [Guid.NewGuid()]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenBranchIdsIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName: "johndoe",
            Password: "Password123!",
            Email: "john@example.com",
            DisplayName: "John Doe",
            PhoneNumber: null,
            PhotoUrl: null,
            BranchIds: [],
            RoleIds: [Guid.NewGuid()]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BranchIds)
            .WithErrorMessage("Debe seleccionar al menos una sucursal.");
    }
}

