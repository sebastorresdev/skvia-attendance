using Skvia.Attendance.Application.Features.Users.DTOs;

namespace Skvia.Attendance.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IUserAccountService userAccountService)
    : IQueryHandler<GetUsersQuery, ErrorOr<List<UserResponse>>>
{
    public Task<ErrorOr<List<UserResponse>>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken)
        => userAccountService.GetUsersAsync(cancellationToken);
}
