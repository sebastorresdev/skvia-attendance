using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Domain.Branches;

namespace Skvia.Attendance.Domain.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        UserClaims = new HashSet<ApplicationUserClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
        Logins = new HashSet<ApplicationUserLogin>();
        Tokens = new HashSet<ApplicationUserToken>();
        BranchUsers = new HashSet<BranchUser>();
    }

    public string? BranchId { get; set; }
    public virtual Branch? Branch { get; set; }

    public virtual ICollection<ApplicationUserClaim> UserClaims { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
    public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
    public ICollection<BranchUser> BranchUsers { get; set; } 
    public string DisplayName { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool IsArchived { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
