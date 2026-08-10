using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Infrastructure.Data.Configurations;

public class EmployeeSchedulesConfiguration : IEntityTypeConfiguration<EmployeeSchedule>
{
    public void Configure(EntityTypeBuilder<EmployeeSchedule> builder)
    {
        builder.ToTable("employee_schedules");

        builder.HasKey(es => es.Id);
        builder.Property(es => es.Id).ValueGeneratedNever();

        builder.Property(es => es.DayType)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(s => new { s.EmployeeId, s.Date }).IsUnique();

        builder.HasOne(es => es.Employee)
            .WithMany(e => e.EmployeeSchedules)
            .HasForeignKey(es => es.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
