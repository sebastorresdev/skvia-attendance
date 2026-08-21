using FluentAssertions;
using Skvia.Erp.Domain.Justifications;

namespace Skvia.Erp.Domain.Tests;

public class JustificationTests
{
    [Fact]
    public void Create_WhenValidParameters_ShouldReturnPendingJustification()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var date = new DateOnly(2026, 8, 8);
        var type = JustificationType.Absence;
        var reason = " Cita médica programada ";
        var documentUrl = " https://example.com/doc.pdf ";

        // Act
        var justification = Justification.Create(employeeId, date, type, reason, documentUrl);

        // Assert
        justification.Should().NotBeNull();
        justification.EmployeeId.Should().Be(employeeId);
        justification.Date.Should().Be(date);
        justification.Type.Should().Be(type);
        justification.Reason.Should().Be("Cita médica programada");
        justification.DocumentUrl.Should().Be("https://example.com/doc.pdf");
        justification.Status.Should().Be(JustificationStatus.Pending);
    }

    [Fact]
    public void Create_WhenEmployeeIdIsEmpty_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var emptyEmployeeId = Guid.Empty;

        // Act
        Action act = () => Justification.Create(emptyEmployeeId, new DateOnly(2026, 8, 8), JustificationType.Tardiness, "Motivos personales");

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenReasonIsInvalid_ShouldThrowArgumentException(string? invalidReason)
    {
        // Act
        Action act = () => Justification.Create(Guid.NewGuid(), new DateOnly(2026, 8, 8), JustificationType.Tardiness, invalidReason!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Approve_WhenCalled_ShouldSetStatusToApprovedAndRecordReviewerInfo()
    {
        // Arrange
        var justification = Justification.Create(Guid.NewGuid(), new DateOnly(2026, 8, 8), JustificationType.Absence, "Falta por salud");
        var reviewerUserId = Guid.NewGuid().ToString();

        // Act
        justification.Approve(reviewerUserId, " Aprobado correctamente ");

        // Assert
        justification.Status.Should().Be(JustificationStatus.Approved);
        justification.ReviewedByUserId.Should().Be(reviewerUserId);
        justification.ReviewerNotes.Should().Be("Aprobado correctamente");
        justification.ReviewedAt.Should().NotBeNull();
    }

    [Fact]
    public void Reject_WhenCalled_ShouldSetStatusToRejectedAndRecordReviewerInfo()
    {
        // Arrange
        var justification = Justification.Create(Guid.NewGuid(), new DateOnly(2026, 8, 8), JustificationType.Absence, "Falta por salud");
        var reviewerUserId = Guid.NewGuid().ToString();

        // Act
        justification.Reject(reviewerUserId, " Documento no sustentatorio ");

        // Assert
        justification.Status.Should().Be(JustificationStatus.Rejected);
        justification.ReviewedByUserId.Should().Be(reviewerUserId);
        justification.ReviewerNotes.Should().Be("Documento no sustentatorio");
        justification.ReviewedAt.Should().NotBeNull();
    }
}

