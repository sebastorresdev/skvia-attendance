using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Identity.Extensions;
using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Application.Identity.Commands.CreateUser;

public class CreateUserCommandHandler(
    UserManager<ApplicationUser> userManager) : ICommandHandler<CreateUserCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
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

        IdentityResult result = await userManager.CreateAsync(user, command.Password);

        if(!result.Succeeded)
            return result.ToApplicationError();

        return user.Id;
    }
}
