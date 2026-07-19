using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Users.Extensions;
using Skvia.Attendance.Domain.Branches;
using Skvia.Attendance.Domain.Identity;
using Skvia.Attendance.Infrastructure.Identity.Domain;

namespace Skvia.Attendance.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IApplicationDbContext dbContext) : ICommandHandler<CreateUserCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var newUser = new ApplicationUser
            {
                UserName = command.UserName,
                DisplayName = command.DisplayName,
                IsActive = true,
                IsArchived = false,
                Email = command.Email,
                ProfilePhotoUrl = command.PhotoUrl,
                PhoneNumber = command.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
            };

            IdentityResult result = await userManager.CreateAsync(newUser, command.Password);

            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                return result.ToApplicationError();
            }

            if (command.Roles.Count != 0)
            {
                IdentityResult roleResult = await userManager.AddToRolesAsync(newUser, command.Roles);

                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return roleResult.ToApplicationError();
                }
            }

            if (command.BranchIds.Count != 0)
            {
                foreach (Guid branchId in command.BranchIds)
                {
                    dbContext.BranchUsers.Add(new BranchUser { BranchId = branchId, UserId = newUser.Id });
                }

                await dbContext.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync();

            return newUser.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            // opcional: loguear ex.Message

            return UserErrors.UnexpectedError(ex.Message);
        }

    }
}
