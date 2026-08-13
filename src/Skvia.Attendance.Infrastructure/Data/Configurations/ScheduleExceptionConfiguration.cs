using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Infrastructure.Data.Configurations;

public class ScheduleExceptionConfiguration : IEntityTypeConfiguration<ScheduleException>
{
    public void Configure(EntityTypeBuilder<ScheduleException> builder)
    {
        builder.ToTable("schedule_exceptions");

        builder.HasKey(se => se.Id);
        builder.Property(se => se.Id).ValueGeneratedNever();

        builder.Property(se => se.DayType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(se => se.Reason)
            .HasMaxLength(250);

        builder.HasIndex(se => new { se.EmployeeId, se.Date }).IsUnique();

        builder.HasOne(se => se.Employee)
            .WithMany()
            .HasForeignKey(se => se.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(se => se.CustomSchedule)
            .WithMany()
            .HasForeignKey(se => se.CustomScheduleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
