using Microsoft.AspNetCore.Identity;

namespace Skvia.Attendance.Domain.Identity;

public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
{
    public string? Description { get; set; }
    public string? Group { get; set; }
    public virtual ApplicationRole Role { get; set; } = default!;
}
