using ErrorOr;

using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Common.Models;
using Skvia.Attendance.Domain.Identity;
using Skvia.Attendance.Infrastructure.Identity.Domain;

namespace Skvia.Attendance.Infrastructure.Identity;

public class UserService(UserManager<ApplicationUser> _userManager) : IUserService
{
    public async Task<ErrorOr<Guid>> CreateUserAsync(string userName, string password, string? email = null, string? displayName = null, string? profilePhotoUrl = null)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            DisplayName = displayName ?? userName,
            IsActive = true,
            IsArchived = false,
            Email = email,
            ProfilePhotoUrl = profilePhotoUrl,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
        };

        IdentityResult result = await _userManager.CreateAsync(user, password);

        if(!result.Succeeded)
            return result.ToApplicationError();

        return user.Id;
    }

    public async Task<ErrorOr<Success>> DeleteUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null) return UserErrors.UserNotFound;

        var result = await _userManager.DeleteAsync(user);

        if(!result.Succeeded) return result.ToApplicationError();

        return Result.Success;
    }

    public async Task<ErrorOr<UserDetailResponse>> GetUserByIdAsync(Guid userId)
    {
        UserDetailResponse? userDetailResponse = await _userManager.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => new UserDetailResponse(
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
            )).FirstOrDefaultAsync();

        return userDetailResponse is null ? UserErrors.UserNotFound : userDetailResponse;
    }

    public async Task<ErrorOr<List<UserResponse>>> GetUsersAsync()
    {
        List<UserResponse> users = await _userManager.Users
            .OrderBy(user => user.NormalizedUserName)
            .Select(user => new UserResponse(
                UserName: user.UserName!,
                IsActive: user.IsActive,
                BranchNames: user.BranchUsers.Select(bu => bu.Branch.Name).ToList(),
                RoleNames: user.UserRoles.Select(ur => ur.Role.Name!).ToList(),
                Email: user.Email,
                PhotoUrl: user.ProfilePhotoUrl,
                LastModifiedAt: user.LastModifiedAt
            )).ToListAsync();

        return users;
    }

    public async Task<ErrorOr<Success>> UpdateUserAsync(Guid userId, string userName, bool isActive, string? email, string? displayName = null, string? profilePhotoUrl = null)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());

        if(user is null) return UserErrors.UserNotFound;

        user.UserName = userName;
        user.IsActive = isActive;
        user.Email = email;
        user.DisplayName = displayName ?? userName;
        user.ProfilePhotoUrl = profilePhotoUrl;
        user.LastModifiedAt = DateTime.UtcNow;

        IdentityResult result = await _userManager.UpdateAsync(user);

        if(!result.Succeeded)
            return result.ToApplicationError();

        return Result.Success;
    }
}
