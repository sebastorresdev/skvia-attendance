using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Domain.Justifications;
using ErrorOr;

namespace Skvia.Erp.Application.Features.Justifications.Commands.CreateJustification;

/// <summary>
/// Comando para registrar o solicitar una nueva justificación.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Justification.Create)]
public record CreateJustificationCommand(
    Guid EmployeeId,
    DateOnly Date,
    JustificationType Type,
    string Reason,
    string? DocumentUrl = null) : ICommand<ErrorOr<Guid>>;


