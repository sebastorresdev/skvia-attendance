using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Features.Employees.Commands.CreateEmployee;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Infrastructure.Data;

namespace Skvia.Attendance.Application.Tests.Features.Employees;

public class CreateEmployeeCommandHandlerTests
{
    private static ApplicationDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task HandleAsync_WhenEmployeeIsUnique_ShouldCreateEmployeeAndReturnId()
    {
        // Arrange
        await using var dbContext = CreateInMemoryDbContext();
        var handler = new CreateEmployeeCommandHandler(dbContext);
        var command = new CreateEmployeeCommand(
            Code: "EMP001",
            FirstName: "Juan",
            LastName: "Pérez",
            DocumentType: DocumentType.Dni,
            DocumentNumber: "12345678",
            HireDate: DateTimeOffset.UtcNow,
            Email: "juan.perez@example.com");

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        var createdEmployee = await dbContext.Employees.FirstOrDefaultAsync(e => e.Id == result.Value);
        createdEmployee.Should().NotBeNull();
        createdEmployee!.Code.Should().Be("EMP001");
        createdEmployee.FirstName.Should().Be("Juan");
    }

    [Fact]
    public async Task HandleAsync_WhenCodeAlreadyExists_ShouldReturnDuplicateCodeError()
    {
        // Arrange
        await using var dbContext = CreateInMemoryDbContext();
        var existing = Employee.Create("EMP001", "María", "López", DocumentIdentifier.Create(DocumentType.Dni, "87654321"), DateTimeOffset.UtcNow);
        dbContext.Employees.Add(existing);
        await dbContext.SaveChangesAsync();

        var handler = new CreateEmployeeCommandHandler(dbContext);
        var command = new CreateEmployeeCommand(
            Code: "emp001", // Duplicate case-insensitive
            FirstName: "Juan",
            LastName: "Pérez",
            DocumentType: DocumentType.Dni,
            DocumentNumber: "12345678",
            HireDate: DateTimeOffset.UtcNow);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(EmployeeErrors.CodeExists("emp001").Code);
    }

    [Fact]
    public async Task HandleAsync_WhenDocumentAlreadyExists_ShouldReturnDuplicateDocumentError()
    {
        // Arrange
        await using var dbContext = CreateInMemoryDbContext();
        var existingDoc = DocumentIdentifier.Create(DocumentType.Dni, "12345678");
        var existing = Employee.Create("EMP001", "María", "López", existingDoc, DateTimeOffset.UtcNow);
        dbContext.Employees.Add(existing);
        await dbContext.SaveChangesAsync();

        var handler = new CreateEmployeeCommandHandler(dbContext);
        var command = new CreateEmployeeCommand(
            Code: "EMP002",
            FirstName: "Juan",
            LastName: "Pérez",
            DocumentType: DocumentType.Dni,
            DocumentNumber: "12345678", // Duplicate document
            HireDate: DateTimeOffset.UtcNow);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(EmployeeErrors.DocumentExists("12345678").Code);
    }
}
