using Microsoft.AspNetCore.Identity;

namespace Skvia.Attendance.Domain.Identity;

public class ApplicationUserClaim : IdentityUserClaim<Guid>
{
    public string? Description { get; set; }
    public virtual ApplicationUser User { get; set; } = default!;
}
