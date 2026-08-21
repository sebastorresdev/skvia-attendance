using Skvia.Erp.Domain.Justifications;

namespace Skvia.Erp.Application.Features.Justifications.DTOs;

public record JustificationResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeCode,
    string BranchName,
    DateOnly Date,
    JustificationType Type,
    string Reason,
    string? DocumentUrl,
    JustificationStatus Status,
    string? ReviewerNotes,
    DateTimeOffset? ReviewedAt,
    DateTimeOffset CreatedAt);

