using Microsoft.AspNetCore.Identity;

namespace Skvia.Erp.Domain.Identity;

public class ApplicationUserLogin : IdentityUserLogin<Guid>
{
    public virtual ApplicationUser User { get; set; } = default!;
}

