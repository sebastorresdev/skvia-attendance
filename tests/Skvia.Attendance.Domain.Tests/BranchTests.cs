using FluentAssertions;
using Skvia.Attendance.Domain.Branches;

namespace Skvia.Attendance.Domain.Tests;

public class BranchTests
{
    [Fact]
    public void Create_WhenValidParameters_ShouldReturnBranch()
    {
        // Arrange
        var code = "lim-01";
        var name = " Sede Principal Lima ";
        var address = " Av. Central 123 ";

        // Act
        var branch = Branch.Create(code, name, address);

        // Assert
        branch.Should().NotBeNull();
        branch.Code.Should().Be("LIM-01");
        branch.Name.Should().Be("Sede Principal Lima");
        branch.Address.Should().Be("Av. Central 123");
    }

    [Fact]
    public void Update_WhenValidParameters_ShouldUpdateBranchDetails()
    {
        // Arrange
        var branch = Branch.Create("LIM-01", "Sede Lima");

        // Act
        branch.Update("LIM-02", "Sede Miraflores", "Av. Larco 456");

        // Assert
        branch.Code.Should().Be("LIM-02");
        branch.Name.Should().Be("Sede Miraflores");
        branch.Address.Should().Be("Av. Larco 456");
    }
}
