using Skvia.Attendance.Application.Common.Models;

namespace Skvia.Attendance.Application.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(ICurrentUserProvider currentUserProvider) : IQueryHandler<GetCurrentUserQuery, ErrorOr<CurrentUser>>
{
    public async Task<ErrorOr<CurrentUser>> HandleAsync(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        return currentUserProvider.GetCurrentUser();
    }
}
