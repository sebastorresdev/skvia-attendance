using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
using Skvia.Erp.Application.Features.Users.DTOs;

namespace Skvia.Erp.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserAccountService userAccountService)
    : IQueryHandler<GetUserByIdQuery, ErrorOr<UserDetailResponse>>
{
    public Task<ErrorOr<UserDetailResponse>> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
        => userAccountService.GetUserByIdAsync(query.UserId, cancellationToken);
}


