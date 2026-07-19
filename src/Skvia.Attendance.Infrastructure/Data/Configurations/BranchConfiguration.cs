using Skvia.Attendance.Domain.Branches;

namespace Skvia.Attendance.Infrastructure.Data.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).IsRequired().ValueGeneratedNever();

        builder.Property(b => b.Code).IsRequired().HasMaxLength(BranchConstants.CodeMaxLength);
        builder.HasIndex(b => b.Code).IsUnique();

        builder.Property(b => b.Name).IsRequired().HasMaxLength(BranchConstants.NameMaxLength);

        builder.Property(p => p.Address).HasMaxLength(BranchConstants.AddressMaxLength);
    }
}
