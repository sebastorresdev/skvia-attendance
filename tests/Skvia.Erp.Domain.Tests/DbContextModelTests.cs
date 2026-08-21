using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Infrastructure.Data;

namespace Skvia.Erp.Domain.Tests;

public class DbContextModelTests
{
    [Fact]
    public void OnModelCreating_BuildsModelWithoutExceptions()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=test_db;Username=postgres;Password=postgres")
            .Options;

        using var dbContext = new ApplicationDbContext(options);

        // Accessing dbContext.Model forces OnModelCreating and model validation to run
        var model = dbContext.Model;

        Assert.NotNull(model);
        var employeeEntity = model.FindEntityType(typeof(Domain.Employees.Employee));
        Assert.NotNull(employeeEntity);
    }

    [Fact]
    public void Employee_ShouldHaveUniqueIndexOnApplicationUserId()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=test_db;Username=postgres;Password=postgres")
            .Options;

        using var dbContext = new ApplicationDbContext(options);
        var employeeEntity = dbContext.Model.FindEntityType(typeof(Domain.Employees.Employee));

        Assert.NotNull(employeeEntity);

        var appUserIndex = employeeEntity!
            .GetIndexes()
            .SingleOrDefault(index => index.Properties.Any(property => property.Name == nameof(Domain.Employees.Employee.ApplicationUserId)));

        Assert.NotNull(appUserIndex);
        Assert.True(appUserIndex!.IsUnique);
    }
}

