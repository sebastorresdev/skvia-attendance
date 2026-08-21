using ErrorOr;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Security;

namespace Skvia.Erp.Application.Features.Employees.Commands.DeleteEmployee;

/// <summary>
/// Comando para eliminar un empleado por su ID.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Employee.Delete)]
public record DeleteEmployeeCommand(Guid EmployeeId) : ICommand<ErrorOr<Success>>;

