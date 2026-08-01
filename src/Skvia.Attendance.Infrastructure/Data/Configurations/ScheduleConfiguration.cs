using Skvia.Attendance.Domain.Schedules;

namespace Skvia.Attendance.Infrastructure.Data.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.ToTable("schedules");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.DefaultStartTime).IsRequired();
        builder.Property(s => s.DefaultEndTime).IsRequired();
    }
}
