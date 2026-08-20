using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Features.Employees.Queries.GetEmployees;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Infrastructure.Data;
using Xunit;

namespace Skvia.Attendance.Application.Tests.Features.Employees;

public class GetEmployeesQueryHandlerTests
{
    private static ApplicationDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmployeeList()
    {
        // Arrange
        await using var dbContext = CreateInMemoryDbContext();
        var employee = Employee.Create(
            code: "EMP001",
            firstName: "Juan",
            lastName: "Pérez",
            documentIdentifier: DocumentIdentifier.Create(DocumentType.Dni, "12345678"),
            hireDate: DateTimeOffset.UtcNow,
            email: "juan.perez@skvia.pe",
            phone: "+51999888777");

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        var handler = new GetEmployeesQueryHandler(dbContext);
        var query = new GetEmployeesQuery();

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(1);
        result.Value[0].Code.Should().Be("EMP001");
        result.Value[0].Email.Should().Be("juan.perez@skvia.pe");
        result.Value[0].Phone.Should().Be("+51999888777");
    }
}
