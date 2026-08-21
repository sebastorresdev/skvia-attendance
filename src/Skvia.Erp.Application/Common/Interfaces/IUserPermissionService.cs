using Skvia.Erp.Domain.Identity;

namespace Skvia.Erp.Application.Common.Interfaces;

public interface IUserPermissionService
{
    Task<List<string>> GetPermissionsAsync(ApplicationUser user);
}

