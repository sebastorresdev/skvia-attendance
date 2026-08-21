using ErrorOr;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Features.Branches.DTOs;

namespace Skvia.Erp.Application.Features.Branches.Queries.GetBranches;

/// <summary>
/// Consulta para obtener el listado de sedes/sucursales.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Branch.View)]
public record GetBranchesQuery() : IQuery<ErrorOr<List<BranchResponse>>>;


