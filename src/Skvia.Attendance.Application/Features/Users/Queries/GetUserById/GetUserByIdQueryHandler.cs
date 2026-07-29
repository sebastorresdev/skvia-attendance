using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Features.Users.DTOs;
using Skvia.Attendance.Domain.Identity;
using Skvia.Attendance.Infrastructure.Identity.Domain;

namespace Skvia.Attendance.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager) : IQueryHandler<GetUserByIdQuery, ErrorOr<UserDetailResponse>>
{
    public async Task<ErrorOr<UserDetailResponse>> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        UserDetailResponse? user = await userManager.Users
            .AsNoTracking()
            .Where(user => user.Id == query.UserId)
            .Select(user => new UserDetailResponse(
                UserId: user.Id,
                DisplayName: user.DisplayName,
                UserName: user.UserName!,
                IsActive: user.IsActive,
                BranchIds: user.BranchUsers.Select(branchUser => branchUser.Branch.Id).ToList(),
                RoleIds: user.UserRoles.Select(ur => ur.Role.Id).ToList(),
                Email: user.Email,
                PhotoUrl: user.ProfilePhotoUrl,
                PhoneNumber: user.PhoneNumber,
                CreatedAt: user.CreatedAt,
                LastModifiedAt: user.LastModifiedAt
            )).FirstOrDefaultAsync(cancellationToken);

        return user is null ? UserErrors.UserNotFound : user;
    }
}
