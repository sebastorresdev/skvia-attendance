using Skvia.Erp.Domain.Common;
using Skvia.Erp.Domain.Employees;

namespace Skvia.Erp.Domain.Justifications;

public class Justification : BaseAuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public DateOnly Date { get; private set; }
    public JustificationType Type { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? DocumentUrl { get; private set; }
    public JustificationStatus Status { get; private set; }
    public string? ReviewerNotes { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public string? ReviewedByUserId { get; private set; }

    private Justification() { }

    public static Justification Create(
        Guid employeeId,
        DateOnly date,
        JustificationType type,
        string reason,
        string? documentUrl = null)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(employeeId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        return new Justification
        {
            EmployeeId = employeeId,
            Date = date,
            Type = type,
            Reason = reason.Trim(),
            DocumentUrl = documentUrl?.Trim(),
            Status = JustificationStatus.Pending
        };
    }

    public void Approve(string reviewerUserId, string? notes = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reviewerUserId);
        Status = JustificationStatus.Approved;
        ReviewedByUserId = reviewerUserId;
        ReviewerNotes = notes?.Trim();
        ReviewedAt = DateTimeOffset.UtcNow;
    }

    public void Reject(string reviewerUserId, string? notes = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reviewerUserId);
        Status = JustificationStatus.Rejected;
        ReviewedByUserId = reviewerUserId;
        ReviewerNotes = notes?.Trim();
        ReviewedAt = DateTimeOffset.UtcNow;
    }
}


