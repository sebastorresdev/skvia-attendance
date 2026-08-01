using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Common.Constants;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Common.Security.Permissions;
using Skvia.Attendance.Application.Features.Auth.Commands.Login;
using Skvia.Attendance.Application.Features.Users.Commands.CreateUser;
using Skvia.Attendance.Application.Features.Users.Commands.DeleteUser;
using Skvia.Attendance.Application.Features.Users.Commands.ResetPassword;
using Skvia.Attendance.Application.Features.Users.Commands.SetUserPermissionOverrides;
using Skvia.Attendance.Application.Features.Users.Commands.UpdateUser;
using Skvia.Attendance.Application.Features.Users.DTOs;
using Skvia.Attendance.Application.Features.Users.Extensions;
using Skvia.Attendance.Domain.Branches;
using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Infrastructure.Services;

public class IdentityUserAccountService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<ApplicationRole> roleManager,
    IApplicationDbContext dbContext) : IUserAccountService
{
    public async Task<ErrorOr<Guid>> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        try
        {
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

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
                    return result.ToApplicationError();
                }

                if (command.RoleIds.Count != 0)
                {
                    var userRoles = command.RoleIds.Select(roleId => new ApplicationUserRole
                    {
                        RoleId = roleId,
                        UserId = newUser.Id
                    });

                    dbContext.ApplicationUserRole.AddRange(userRoles);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                if (command.BranchIds.Count != 0)
                {
                    foreach (Guid branchId in command.BranchIds)
                    {
                        dbContext.BranchUsers.Add(new BranchUser
                        {
                            BranchId = branchId,
                            UserId = newUser.Id
                        });
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return (ErrorOr<Guid>)newUser.Id;
            });
        }
        catch (Exception ex)
        {
            return UserErrors.UnexpectedError(ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        ApplicationUser? existingUser = await userManager.FindByIdAsync(command.UserId.ToString());
        if (existingUser is null)
        {
            return UserErrors.UserNotFound;
        }

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

            await dbContext.SaveChangesAsync(cancellationToken);
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
        {
            return result.ToApplicationError();
        }

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

    public async Task<ErrorOr<Success>> ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        if (command.NewPassword != command.ConfirmNewPassword)
        {
            return Error.Conflict("Las contraseñas no coinciden.");
        }

        IdentityResult removeResult = await userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            var errors = string.Join("; ", removeResult.Errors.Select(e => e.Description));
            return Error.Conflict($"No se pudo eliminar la contraseña actual: {errors}");
        }

        IdentityResult addResult = await userManager.AddPasswordAsync(user, command.NewPassword);
        if (!addResult.Succeeded)
        {
            var errors = string.Join("; ", addResult.Errors.Select(e => e.Description));
            return Error.Conflict($"No se pudo asignar la nueva contraseña: {errors}");
        }

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> DeleteUserAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var affectedRows = await userManager.Users
            .Where(u => command.UserIds.Contains(u.Id))
            .ExecuteDeleteAsync(cancellationToken);

        if (affectedRows <= 0)
        {
            return Error.Conflict("No se pudo eliminar los usuarios");
        }

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> SetPermissionOverridesAsync(SetUserPermissionOverridesCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
        {
            return Error.NotFound("User.NotFound", "Usuario no encontrado");
        }

        var currentClaims = await userManager.GetClaimsAsync(user);
        var currentPermissionClaims = currentClaims.Where(c => c.Type == CustomClaimTypes.Permission).ToList();

        if (currentPermissionClaims.Count > 0)
        {
            IdentityResult removeResult = await userManager.RemoveClaimsAsync(user, currentPermissionClaims);
            if (!removeResult.Succeeded)
            {
                return Error.Failure("Permissions.RemoveFailed", "No se pudieron limpiar los permisos actuales");
            }
        }

        var newClaims = command.PermissionKeys.Select(key => new Claim(CustomClaimTypes.Permission, key)).ToList();
        if (newClaims.Count > 0)
        {
            IdentityResult addResult = await userManager.AddClaimsAsync(user, newClaims);
            if (!addResult.Succeeded)
            {
                return Error.Failure("Permissions.AddFailed", "No se pudieron asignar los nuevos permisos");
            }
        }

        return Result.Success;
    }

    public async Task<ErrorOr<ClaimsPrincipal>> AuthenticateAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(command.UserName);
        if (user is null)
        {
            return Error.Unauthorized("Credenciales Invalidas.");
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            return Error.Unauthorized("Usuario Bloqueado.");
        }

        if (!user.IsActive)
        {
            return Error.Unauthorized("Tu cuenta está inactiva. Ponte en contacto con el servicio de asistencia para obtener ayuda.");
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, command.Password);
        if (!isPasswordValid)
        {
            await userManager.AccessFailedAsync(user);
            if (await userManager.IsLockedOutAsync(user))
            {
                return Error.Unauthorized("La cuenta ha sido bloqueada debido a múltiples intentos fallidos de inicio de sesión.");
            }

            return Error.Unauthorized("El nombre de usuario o la contraseña son incorrectos. Inténtalo de nuevo.");
        }

        return await signInManager.CreateUserPrincipalAsync(user);
    }

    public async Task<ErrorOr<UserDetailResponse>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        UserDetailResponse? user = await userManager.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => new UserDetailResponse(
                Id: user.Id,
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
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return user is null ? UserErrors.UserNotFound : user;
    }

    public async Task<ErrorOr<List<UserResponse>>> GetUsersAsync(CancellationToken cancellationToken)
    {
        List<UserResponse> users = await userManager.Users
            .OrderBy(user => user.NormalizedUserName)
            .Select(user => new UserResponse(
                Id: user.Id,
                UserName: user.UserName!,
                IsActive: user.IsActive,
                BranchName: user.BranchUsers.Select(bu => bu.Branch.Name).First(),
                RoleNames: user.UserRoles.Select(ur => ur.Role.Name!).ToList(),
                Email: user.Email,
                PhotoUrl: user.ProfilePhotoUrl,
                LastModifiedAt: user.LastModifiedAt
            ))
            .ToListAsync(cancellationToken);

        return users;
    }

    public async Task<ErrorOr<List<PermissionGroupResponse>>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Error.NotFound("User.NotFound", "Usuario no encontrado");
        }

        var roleNames = await userManager.GetRolesAsync(user);
        var rolePermissionKeys = new HashSet<string>();

        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                continue;
            }

            var roleClaims = await roleManager.GetClaimsAsync(role);
            foreach (var claim in roleClaims.Where(c => c.Type == CustomClaimTypes.Permission))
            {
                rolePermissionKeys.Add(claim.Value);
            }
        }

        var userClaims = await userManager.GetClaimsAsync(user);
        var overrideKeys = userClaims.Where(c => c.Type == CustomClaimTypes.Permission).Select(c => c.Value).ToHashSet();

        var catalog = PermissionCatalog.GetAll();
        var result = catalog.Select(g => new PermissionGroupResponse(
            g.Group,
            g.GroupDescription,
            g.Permissions.Select(p =>
            {
                var fromRole = rolePermissionKeys.Contains(p.Key);
                var fromOverride = overrideKeys.Contains(p.Key);

                return new PermissionItemResponse(
                    p.Key,
                    p.Display,
                    p.Description,
                    Granted: fromRole || fromOverride,
                    Source: fromRole ? "Role" : fromOverride ? "Override" : null);
            }).ToList()
        )).ToList();

        return result;
    }
}
