using Microsoft.AspNetCore.Identity;

namespace Skvia.Erp.Domain.Identity;

public class ApplicationUserToken : IdentityUserToken<Guid>
{
    public virtual ApplicationUser User { get; set; } = default!;
}

