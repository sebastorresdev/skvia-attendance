using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Features.Users.DTOs;

namespace Skvia.Erp.Application.Features.Users.Queries.GetUserById;

/// <summary>
/// Consulta para obtener el detalle de un usuario por ID.
/// </summary>
[AuthorizeCommand(Permissions = Permission.User.View)]
public record GetUserByIdQuery(Guid UserId) : IQuery<ErrorOr<UserDetailResponse>>;



