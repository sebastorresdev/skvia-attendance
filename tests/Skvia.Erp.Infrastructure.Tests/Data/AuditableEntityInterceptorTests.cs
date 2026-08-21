using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Features.Auth.DTOs;
using Skvia.Erp.Domain.Employees;
using Skvia.Erp.Infrastructure.Data;
using Skvia.Erp.Infrastructure.Data.Interceptors;

namespace Skvia.Erp.Infrastructure.Tests.Data;

public class AuditableEntityInterceptorTests
{
    private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();

    private ApplicationDbContext CreateDbContextWithInterceptor()
    {
        var interceptor = new AuditableEntityInterceptor(_currentUserProvider);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .AddInterceptors(interceptor)
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenEntityAdded_ShouldSetCreatedAndCreatedBy()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserProvider.GetCurrentUser().Returns(new CurrentUserResponse(userId, ["Admin"], ["read", "write"]));

        await using var dbContext = CreateDbContextWithInterceptor();
        var employee = Employee.Create("EMP100", "Ana", "Torres", DocumentIdentifier.Create(DocumentType.Dni, "11223344"), DateTimeOffset.UtcNow);

        // Act
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        // Assert
        employee.CreatedBy.Should().Be(userId);
        employee.Created.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task SavingChangesAsync_WhenEntityModified_ShouldSetLastModifiedAndLastModifiedBy()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserProvider.GetCurrentUser().Returns(new CurrentUserResponse(userId, ["Admin"], ["read", "write"]));

        await using var dbContext = CreateDbContextWithInterceptor();
        var employee = Employee.Create("EMP100", "Ana", "Torres", DocumentIdentifier.Create(DocumentType.Dni, "11223344"), DateTimeOffset.UtcNow);
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        var modifierUserId = Guid.NewGuid();
        _currentUserProvider.GetCurrentUser().Returns(new CurrentUserResponse(modifierUserId, ["Editor"], ["edit"]));

        // Act
        employee.Update("EMP100", "Ana María", "Torres", DocumentIdentifier.Create(DocumentType.Dni, "11223344"), DateTimeOffset.UtcNow);
        await dbContext.SaveChangesAsync();

        // Assert
        employee.LastModifiedBy.Should().Be(modifierUserId);
        employee.LastModified.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task SavingChangesAsync_WhenCurrentUserThrowsInvalidOperationException_ShouldDefaultToEmptyGuid()
    {
        // Arrange
        _currentUserProvider.GetCurrentUser().Returns(_ => throw new InvalidOperationException("No HTTP context"));

        await using var dbContext = CreateDbContextWithInterceptor();
        var employee = Employee.Create("EMP100", "Ana", "Torres", DocumentIdentifier.Create(DocumentType.Dni, "11223344"), DateTimeOffset.UtcNow);

        // Act
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        // Assert
        employee.CreatedBy.Should().Be(Guid.Empty);
    }
}

