using FluentValidation.TestHelper;
using Skvia.Attendance.Application.Features.Employees.Commands.CreateEmployee;
using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Tests.Features.Employees;

public class CreateEmployeeCommandValidatorTests
{
    private readonly CreateEmployeeCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationErrors()
    {
        // Arrange
        var command = new CreateEmployeeCommand(
            Code: "EMP001",
            FirstName: "Juan",
            LastName: "Pérez",
            DocumentType: DocumentType.Dni,
            DocumentNumber: "12345678",
            HireDate: DateTimeOffset.UtcNow,
            Email: "juan@example.com",
            Phone: "+51 987654321",
            Position: "Developer",
            Department: "IT",
            PhotoUrl: "https://example.com/photo.jpg");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenCodeIsEmpty_ShouldHaveValidationErrorForCode(string? code)
    {
        // Arrange
        var command = CreateValidCommand() with { Code = code! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Code)
            .WithErrorMessage("El código del empleado es obligatorio.");
    }

    [Fact]
    public void Validate_WhenCodeHasInvalidCharacters_ShouldHaveValidationErrorForCode()
    {
        // Arrange
        var command = CreateValidCommand() with { Code = "EMP 001!" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Code)
            .WithErrorMessage("El código solo puede contener letras, números, guiones o guiones bajos.");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    public void Validate_WhenEmailIsInvalid_ShouldHaveValidationErrorForEmail(string invalidEmail)
    {
        // Arrange
        var command = CreateValidCommand() with { Email = invalidEmail };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El formato del correo electrónico no es válido.");
    }

    [Fact]
    public void Validate_WhenPhotoUrlIsNotAbsolute_ShouldHaveValidationErrorForPhotoUrl()
    {
        // Arrange
        var command = CreateValidCommand() with { PhotoUrl = "relative/path/photo.png" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhotoUrl);
    }

    private static CreateEmployeeCommand CreateValidCommand() => new(
        Code: "EMP001",
        FirstName: "Juan",
        LastName: "Pérez",
        DocumentType: DocumentType.Dni,
        DocumentNumber: "12345678",
        HireDate: DateTimeOffset.UtcNow);
}
