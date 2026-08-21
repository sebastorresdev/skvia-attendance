using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skvia.Erp.Domain.EmployeeSchedules;

namespace Skvia.Erp.Infrastructure.Data.Configurations;

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

        builder.HasIndex(es => new { es.EmployeeId, es.EffectiveFrom });

        builder.HasOne(es => es.Employee)
            .WithMany(e => e.EmployeeSchedules)
            .HasForeignKey(es => es.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(es => es.Schedule)
            .WithMany()
            .HasForeignKey(es => es.ScheduleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

