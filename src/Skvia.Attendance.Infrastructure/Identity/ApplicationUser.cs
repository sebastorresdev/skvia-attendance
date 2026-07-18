using Microsoft.AspNetCore.Identity;

namespace Skvia.Attendance.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool IsArchived { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
