using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Users.Extensions;
using Skvia.Attendance.Domain.Identity;
using Skvia.Attendance.Infrastructure.Identity.Domain;

namespace Skvia.Attendance.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(UserManager<ApplicationUser> userManager) : ICommandHandler<DeleteUserCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteUserCommand query, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());

        if (user is null) return UserErrors.UserNotFound;

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded) return result.ToApplicationError();

        return Result.Success;
    }
}
