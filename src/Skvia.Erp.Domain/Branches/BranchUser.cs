using Skvia.Erp.Domain.Common;
using Skvia.Erp.Domain.Identity;

namespace Skvia.Erp.Domain.Branches;

public class BranchUser : BaseAuditableEntity
{
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}


