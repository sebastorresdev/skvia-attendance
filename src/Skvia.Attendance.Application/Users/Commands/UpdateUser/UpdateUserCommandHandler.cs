using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Users.Extensions;
using Skvia.Attendance.Domain.Branches;
using Skvia.Attendance.Domain.Identity;
using Skvia.Attendance.Infrastructure.Identity.Domain;

namespace Skvia.Attendance.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IApplicationDbContext dbContext) : ICommandHandler<UpdateUserCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        ApplicationUser? existingUser = await userManager.FindByIdAsync(command.UserId.ToString());

        if (existingUser is null) return UserErrors.UserNotFound;

        IList<string> existingRoles = await userManager.GetRolesAsync(existingUser);

        if (existingRoles.Any())
        {
            await userManager.RemoveFromRolesAsync(existingUser, existingRoles);
        }

        List<BranchUser> branchUsers = await dbContext.BranchUsers.Where(x => x.UserId == existingUser.Id).ToListAsync(cancellationToken);
        if (branchUsers.Count != 0)
        {
            dbContext.BranchUsers.RemoveRange(branchUsers);
        }

        if (command.BranchIds.Count != 0)
        {
            foreach (Guid branchId in command.BranchIds)
            {
                dbContext.BranchUsers.Add(new BranchUser { BranchId = branchId, UserId = existingUser.Id });
            }
            await dbContext.SaveChangesAsync(default);
        }

        existingUser.UserName = command.UserName;
        existingUser.IsActive = command.IsActive;
        existingUser.Email = command.Email;
        existingUser.DisplayName = command.DisplayName;
        existingUser.PhoneNumber = command.PhoneNumber;
        existingUser.ProfilePhotoUrl = command.PhotoUrl;
        existingUser.LastModifiedAt = DateTime.UtcNow;

        IdentityResult result = await userManager.UpdateAsync(existingUser);

        if (!result.Succeeded)
            return result.ToApplicationError();

        if (command.RoleIds.Count != 0)
        {
            var userRoles = command.RoleIds.Select(roleId => new ApplicationUserRole
            {
                RoleId = roleId,
                UserId = existingUser.Id
            });

            dbContext.ApplicationUserRole.AddRange(userRoles);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success;
    }
}
