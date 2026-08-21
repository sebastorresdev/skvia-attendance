using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Features.Departments.DTOs;

using ErrorOr;

namespace Skvia.Erp.Application.Features.Departments.Queries.GetDepartments;

/// <summary>
/// Consulta para obtener el listado de departamentos.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Department.View)]
public record GetDepartmentsQuery() : IQuery<ErrorOr<List<DepartmentResponse>>>;



