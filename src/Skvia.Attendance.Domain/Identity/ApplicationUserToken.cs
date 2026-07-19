using Microsoft.AspNetCore.Identity;

namespace Skvia.Attendance.Domain.Identity;

public class ApplicationUserToken : IdentityUserToken<Guid>
{
    public virtual ApplicationUser User { get; set; } = default!;
}
