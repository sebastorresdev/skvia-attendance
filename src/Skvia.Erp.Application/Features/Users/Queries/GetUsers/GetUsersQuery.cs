using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Features.Users.DTOs;

namespace Skvia.Erp.Application.Features.Users.Queries.GetUsers;

/// <summary>
/// Consulta para obtener el listado de usuarios del sistema.
/// </summary>
[AuthorizeCommand(Permissions = Permission.User.View)]
public record GetUsersQuery() : IQuery<ErrorOr<List<UserResponse>>>;



