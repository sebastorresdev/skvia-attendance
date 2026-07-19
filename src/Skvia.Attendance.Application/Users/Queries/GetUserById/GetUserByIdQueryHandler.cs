using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Users.DTOs;
using Skvia.Attendance.Domain.Identity;
using Skvia.Attendance.Infrastructure.Identity.Domain;

namespace Skvia.Attendance.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager) : IQueryHandler<GetUserByIdQuery, ErrorOr<UserDetailResponse>>
{
    public async Task<ErrorOr<UserDetailResponse>> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        UserDetailResponse? userDetailResponse = await userManager.Users
            .AsNoTracking()
            .Where(user => user.Id == query.UserId)
            .Select(user => new UserDetailResponse(
                UserId: user.Id,
                DisplayName: user.DisplayName,
                UserName: user.UserName!,
                IsActive: user.IsActive,
                BranchNames: user.BranchUsers.Select(bu => bu.Branch.Name).ToList(),
                RoleNames: user.UserRoles.Select(ur => ur.Role.Name!).ToList(),
                Email: user.Email,
                PhotoUrl: user.ProfilePhotoUrl,
                PhoneNumber: user.PhoneNumber,
                CreatedAt: user.CreatedAt,
                LastModifiedAt: user.LastModifiedAt
            )).FirstOrDefaultAsync(cancellationToken);

        return userDetailResponse is null ? UserErrors.UserNotFound : userDetailResponse;
    }
}
