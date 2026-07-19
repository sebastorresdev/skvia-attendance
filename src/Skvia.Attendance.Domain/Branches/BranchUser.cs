namespace Skvia.Attendance.Domain.Branches;

public class BranchUser
{
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public Guid UserId { get; set; }
}
