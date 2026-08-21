using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
using Skvia.Erp.Application.Features.Auth.DTOs;

namespace Skvia.Erp.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(ICurrentUserProvider currentUserProvider) : IQueryHandler<GetCurrentUserQuery, ErrorOr<CurrentUserResponse>>
{
    public async Task<ErrorOr<CurrentUserResponse>> HandleAsync(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        return currentUserProvider.GetCurrentUser();
    }
}


