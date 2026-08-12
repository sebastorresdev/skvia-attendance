using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skvia.Attendance.Domain.Kiosks;

namespace Skvia.Attendance.Infrastructure.Data.Configurations;

public class KioskDeviceConfiguration : IEntityTypeConfiguration<KioskDevice>
{
    public void Configure(EntityTypeBuilder<KioskDevice> builder)
    {
        builder.ToTable("KioskDevices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.PairingCode)
            .HasMaxLength(10);

        builder.Property(x => x.PairingCodeExpiresAt);

        builder.Property(x => x.LinkedAt);

        builder.HasOne(x => x.Workplace)
            .WithMany()
            .HasForeignKey(x => x.WorkplaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasIndex(x => x.PairingCode);
    }
}

