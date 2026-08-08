using FluentAssertions;
using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Domain.Tests;

public class EmployeeTests
{
    [Fact]
    public void Create_WhenValidParameters_ShouldReturnActiveEmployee()
    {
        // Arrange
        var code = "emp001";
        var firstName = "Juan";
        var lastName = "Pérez";
        var doc = DocumentIdentifier.Create(DocumentType.Dni, "12345678");
        var hireDate = DateTimeOffset.UtcNow;
        var email = "juan.perez@example.com";
        var phone = "+51 987654321";

        // Act
        var employee = Employee.Create(code, firstName, lastName, doc, hireDate, email, phone);

        // Assert
        employee.Should().NotBeNull();
        employee.Code.Should().Be("EMP001");
        employee.FirstName.Should().Be("Juan");
        employee.LastName.Should().Be("Pérez");
        employee.DocumentIdentifier.Should().Be(doc);
        employee.HireDate.Should().Be(hireDate);
        employee.Email.Should().NotBeNull();
        employee.Email!.Value.Value.Should().Be("juan.perez@example.com");
        employee.Phone.Should().NotBeNull();
        employee.Phone!.Value.Value.Should().Be("+51 987654321");
        employee.Status.Should().Be(EmployeeStatus.Active);
    }

    [Fact]
    public void Create_WhenCodeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var doc = DocumentIdentifier.Create(DocumentType.Dni, "12345678");
        var hireDate = DateTimeOffset.UtcNow;

        // Act
        Action act = () => Employee.Create(null!, "Juan", "Pérez", doc, hireDate);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WhenFirstNameExceedsMaxLength_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var doc = DocumentIdentifier.Create(DocumentType.Dni, "12345678");
        var longFirstName = new string('A', EmployeeConstants.FirstNameMaxLength + 1);
        var hireDate = DateTimeOffset.UtcNow;

        // Act
        Action act = () => Employee.Create("EMP001", longFirstName, "Pérez", doc, hireDate);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Update_WhenValidParameters_ShouldUpdateEmployeeProperties()
    {
        // Arrange
        var doc = DocumentIdentifier.Create(DocumentType.Dni, "12345678");
        var employee = Employee.Create("EMP001", "Juan", "Pérez", doc, DateTimeOffset.UtcNow);
        var newDoc = DocumentIdentifier.Create(DocumentType.Ce, "87654321");

        // Act
        employee.Update("EMP002", "Carlos", "Gómez", newDoc, DateTimeOffset.UtcNow, "carlos@example.com");

        // Assert
        employee.Code.Should().Be("EMP002");
        employee.FirstName.Should().Be("Carlos");
        employee.LastName.Should().Be("Gómez");
        employee.DocumentIdentifier.Should().Be(newDoc);
        employee.Email!.Value.Value.Should().Be("carlos@example.com");
    }

    [Fact]
    public void ChangeStatus_WhenCalled_ShouldUpdateEmployeeStatus()
    {
        // Arrange
        var doc = DocumentIdentifier.Create(DocumentType.Dni, "12345678");
        var employee = Employee.Create("EMP001", "Juan", "Pérez", doc, DateTimeOffset.UtcNow);

        // Act
        employee.ChangeStatus(EmployeeStatus.Inactive);

        // Assert
        employee.Status.Should().Be(EmployeeStatus.Inactive);
    }

    [Fact]
    public void LinkUser_WhenValidUserId_ShouldSetApplicationUserId()
    {
        // Arrange
        var doc = DocumentIdentifier.Create(DocumentType.Dni, "12345678");
        var employee = Employee.Create("EMP001", "Juan", "Pérez", doc, DateTimeOffset.UtcNow);
        var userId = Guid.NewGuid().ToString();

        // Act
        employee.LinkUser(userId);

        // Assert
        employee.ApplicationUserId.Should().Be(userId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void LinkUser_WhenInvalidUserId_ShouldThrowArgumentException(string? invalidUserId)
    {
        // Arrange
        var doc = DocumentIdentifier.Create(DocumentType.Dni, "12345678");
        var employee = Employee.Create("EMP001", "Juan", "Pérez", doc, DateTimeOffset.UtcNow);

        // Act
        Action act = () => employee.LinkUser(invalidUserId!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
