using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.EmployeeSchedules.Commands.DeleteScheduleException;

/// <summary>
/// Comando para eliminar una excepción de horario.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Schedule.Delete)]
public record DeleteScheduleExceptionCommand(Guid Id) : ICommand<ErrorOr<Success>>;



