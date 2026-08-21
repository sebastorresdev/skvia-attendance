using ErrorOr;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Features.Employees.DTOs;

namespace Skvia.Erp.Application.Features.Employees.Queries.GetEmployeeById;

/// <summary>
/// Consulta para obtener los detalles completos de un empleado por su ID.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Employee.View)]
public record GetEmployeeByIdQuery(Guid EmployeeId) : IQuery<ErrorOr<EmployeeDetailResponse>>;


