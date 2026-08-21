using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
using Skvia.Erp.Application.Features.Users.DTOs;

namespace Skvia.Erp.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IUserAccountService userAccountService)
    : IQueryHandler<GetUsersQuery, ErrorOr<List<UserResponse>>>
{
    public Task<ErrorOr<List<UserResponse>>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken)
        => userAccountService.GetUsersAsync(cancellationToken);
}


