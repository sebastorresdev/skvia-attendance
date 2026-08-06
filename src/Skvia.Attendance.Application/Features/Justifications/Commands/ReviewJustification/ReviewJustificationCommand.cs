using Skvia.Attendance.Application.Common.Messaging;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Justifications.Commands.ReviewJustification;

public record ReviewJustificationCommand(
    Guid JustificationId,
    bool Approve,
    string? Notes = null) : ICommand<ErrorOr<Success>>;
