using Skvia.Attendance.Application.Common.Models;

namespace Skvia.Attendance.Application.Common.Interfaces;

public interface IUserService
{
    Task<ErrorOr<Guid>> CreateUserAsync(string userName, string password, string? email = null, string? displayName = null, string? profilePhotoUrl = null);
    Task<ErrorOr<Success>> UpdateUserAsync(Guid userId, string userName, bool isActive, string? email = null, string? displayName = null, string? profilePhotoUrl = null);
    Task<ErrorOr<Success>> DeleteUserAsync(Guid userId);
    
    Task<ErrorOr<List<UserResponse>>> GetUsersAsync();
    Task<ErrorOr<UserDetailResponse>> GetUserByIdAsync(Guid userId);
}
