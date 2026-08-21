using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Features.Auth.DTOs;

namespace Skvia.Erp.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery() : IQuery<ErrorOr<CurrentUserResponse>>;


