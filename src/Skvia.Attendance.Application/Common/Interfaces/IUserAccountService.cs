using System.Security.Claims;

using Skvia.Attendance.Application.Features.Auth.Commands.Login;
using Skvia.Attendance.Application.Features.Users.Commands.CreateUser;
using Skvia.Attendance.Application.Features.Users.Commands.DeleteUser;
using Skvia.Attendance.Application.Features.Users.Commands.ResetPassword;
using Skvia.Attendance.Application.Features.Users.Commands.SetUserPermissionOverrides;
using Skvia.Attendance.Application.Features.Users.Commands.UpdateUser;
using Skvia.Attendance.Application.Features.Users.DTOs;

namespace Skvia.Attendance.Application.Common.Interfaces;

public interface IUserAccountService
{
    Task<ErrorOr<Guid>> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> DeleteUserAsync(DeleteUserCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> SetPermissionOverridesAsync(SetUserPermissionOverridesCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<ClaimsPrincipal>> AuthenticateAsync(LoginCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<UserDetailResponse>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<ErrorOr<List<UserResponse>>> GetUsersAsync(CancellationToken cancellationToken);
    Task<ErrorOr<List<PermissionGroupDto>>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken);
}
