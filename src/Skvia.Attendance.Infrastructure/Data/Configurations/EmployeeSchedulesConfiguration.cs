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

        // 🏬 Optimización para cargar el calendario semanal de una tienda específica
        builder.HasIndex(es => new { es.BranchId, es.Date });

        builder.HasIndex(s => new { s.EmployeeId, s.Date }).IsUnique();

        builder.HasOne(es => es.Employee)
            .WithMany(e => e.EmployeeSchedules)
            .HasForeignKey(es => es.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(es => es.Branch)
            .WithMany()
            .HasForeignKey(es => es.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
