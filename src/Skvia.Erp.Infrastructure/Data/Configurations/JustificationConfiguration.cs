using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skvia.Erp.Domain.Justifications;

namespace Skvia.Erp.Infrastructure.Data.Configurations;

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

        builder.HasIndex(j => j.EmployeeId);
        builder.HasIndex(j => j.Status);
        builder.HasIndex(j => new { j.EmployeeId, j.Date });
        builder.HasIndex(j => new { j.Status, j.Date });

        builder.HasOne(j => j.Employee)
            .WithMany()
            .HasForeignKey(j => j.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

