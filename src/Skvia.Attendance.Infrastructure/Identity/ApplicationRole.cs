using Microsoft.AspNetCore.Identity;

namespace Skvia.Attendance.Infrastructure.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
}
