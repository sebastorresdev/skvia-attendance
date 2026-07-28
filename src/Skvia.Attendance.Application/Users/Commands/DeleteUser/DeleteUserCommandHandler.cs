using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(UserManager<ApplicationUser> userManager, IApplicationDbContext dbContext) : ICommandHandler<DeleteUserCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteUserCommand query, CancellationToken cancellationToken)
    {
        var affectedRows = await userManager.Users
            .Where(u => query.UserIds.Contains(u.Id))
            .ExecuteDeleteAsync(cancellationToken);

        if (affectedRows <= 0)
            return Error.Conflict("No se pudo eliminar los usuarios");

        return Result.Success;
    }
}
