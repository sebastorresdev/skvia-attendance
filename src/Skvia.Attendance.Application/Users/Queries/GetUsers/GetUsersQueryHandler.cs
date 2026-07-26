using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Users.DTOs;
using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(UserManager<ApplicationUser> userManager) : IQueryHandler<GetUsersQuery, ErrorOr<List<UserResponse>>>
{
    public async Task<ErrorOr<List<UserResponse>>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken)
    {
        List<UserResponse> users = await userManager.Users
            .OrderBy(user => user.NormalizedUserName)
            .Select(user => new UserResponse(
                UserId: user.Id,
                UserName: user.UserName!,
                IsActive: user.IsActive,
                BranchName: user.BranchUsers.Select(bu => bu.Branch.Name).First(),
                RoleNames: user.UserRoles.Select(ur => ur.Role.Name!).ToList(),
                Email: user.Email,
                PhotoUrl: user.ProfilePhotoUrl,
                LastModifiedAt: user.LastModifiedAt
            )).ToListAsync(cancellationToken);

        return users;
    }
}
