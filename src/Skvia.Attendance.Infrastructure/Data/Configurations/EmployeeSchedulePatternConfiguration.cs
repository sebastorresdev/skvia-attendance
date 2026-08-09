using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Infrastructure.Data.Configurations;

public class EmployeeSchedulePatternConfiguration : IEntityTypeConfiguration<EmployeeSchedulePattern>
{
    public void Configure(EntityTypeBuilder<EmployeeSchedulePattern> builder)
    {
        builder.ToTable("EmployeeSchedulePatterns");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DayOfWeek)
            .IsRequired();

        builder.Property(x => x.IsWorkDay)
            .IsRequired();

        builder.Property(x => x.StartTime)
            .IsRequired(false);

        builder.Property(x => x.EndTime)
            .IsRequired(false);

        builder.HasOne(x => x.Employee)
            .WithMany(e => e.SchedulePatterns)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(x => new { x.EmployeeId, x.DayOfWeek })
            .IsUnique();
    }
}
