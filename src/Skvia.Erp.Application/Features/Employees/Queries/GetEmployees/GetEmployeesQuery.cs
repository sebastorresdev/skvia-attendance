using ErrorOr;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Features.Employees.DTOs;

namespace Skvia.Erp.Application.Features.Employees.Queries.GetEmployees;

/// <summary>
/// Consulta para obtener el listado completo de empleados.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Employee.View)]
public record GetEmployeesQuery() : IQuery<ErrorOr<List<EmployeeResponse>>>;


