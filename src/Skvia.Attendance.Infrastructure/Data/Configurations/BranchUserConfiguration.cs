using Skvia.Attendance.Domain.Branches;

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

        builder.HasOne(bu => bu.User)
            .WithMany(u => u.BranchUsers)
            .HasForeignKey(bu => bu.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
