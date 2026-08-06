using Skvia.Attendance.Application.Common.Messaging;
using Skvia.Attendance.Domain.Justifications;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Justifications.Commands.CreateJustification;

public record CreateJustificationCommand(
    Guid EmployeeId,
    DateOnly Date,
    JustificationType Type,
    string Reason,
    string? DocumentUrl = null) : ICommand<ErrorOr<Guid>>;
