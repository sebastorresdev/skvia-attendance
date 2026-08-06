using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skvia.Attendance.Domain.Justifications;

namespace Skvia.Attendance.Infrastructure.Data.Configurations;

public class JustificationConfiguration : IEntityTypeConfiguration<Justification>
{
    public void Configure(EntityTypeBuilder<Justification> builder)
    {
        builder.HasKey(j => j.Id);

        builder.Property(j => j.Reason)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(j => j.DocumentUrl)
            .HasMaxLength(2048);

        builder.Property(j => j.ReviewerNotes)
            .HasMaxLength(500);

        builder.HasOne(j => j.Employee)
            .WithMany()
            .HasForeignKey(j => j.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
