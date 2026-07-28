using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Application.Common.Interfaces;

public interface IUserPermissionService
{
    Task<List<string>> GetPermissionsAsync(ApplicationUser user);
}
