using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;

namespace Skvia.Erp.Application.Features.Justifications.Commands.ReviewJustification;

public record ReviewJustificationCommand(
    Guid JustificationId,
    bool Approve,
    string? Notes = null) : ICommand<ErrorOr<Success>>;

