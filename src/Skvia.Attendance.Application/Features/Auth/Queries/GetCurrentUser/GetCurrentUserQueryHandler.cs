using Skvia.Attendance.Application.Features.Auth.DTOs;

namespace Skvia.Attendance.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(ICurrentUserProvider currentUserProvider) : IQueryHandler<GetCurrentUserQuery, ErrorOr<CurrentUserResponse>>
{
    public async Task<ErrorOr<CurrentUserResponse>> HandleAsync(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        return currentUserProvider.GetCurrentUser();
    }
}
