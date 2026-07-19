using Skvia.Attendance.Domain.Branches;
using Skvia.Attendance.Infrastructure.Identity;

namespace Skvia.Attendance.Infrastructure.Data.Configurations;

public class UserBranchConfiguration : IEntityTypeConfiguration<BranchUser>
{
    public void Configure(EntityTypeBuilder<BranchUser> builder)
    {
        builder.HasKey(bu => new { bu.UserId, bu.BranchId });

        builder.HasOne(bu => bu.Branch)
            .WithMany(b => b.BranchUsers)
            .HasForeignKey(bu => bu.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(bu => bu.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
