namespace Skvia.Attendance.Infrastructure.Data.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Domain.Attendances.Attendance>
{
    public void Configure(EntityTypeBuilder<Domain.Attendances.Attendance> builder)
    {
        builder.ToTable("attendances");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.Date).IsRequired();
        builder.Property(a => a.CheckIn).IsRequired();
        builder.Property(a => a.PhotoCheckIn).IsRequired().HasMaxLength(500);

        builder.Property(a => a.BreakStart).IsRequired(false);
        builder.Property(a => a.PhotoBreakStart).HasMaxLength(500).IsRequired(false);
        builder.Property(a => a.BreakEnd).IsRequired(false);
        builder.Property(a => a.PhotoBreakEnd).HasMaxLength(500).IsRequired(false);

        builder.Property(a => a.CheckOut).IsRequired(false);
        builder.Property(a => a.PhotoCheckOut).HasMaxLength(500).IsRequired(false);

        builder.Property(a => a.IsLate).IsRequired();
        builder.Property(a => a.MinutesLate).IsRequired();
        builder.Property(a => a.MinutesWorked).IsRequired();
        builder.Property(a => a.OvertimeMinutes).IsRequired();
        builder.Property(a => a.IsValidCheckIn).IsRequired();
        builder.Property(a => a.IsValidCheckOut).IsRequired();

        builder.HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CheckInBranch)
            .WithMany()
            .HasForeignKey(a => a.CheckInBranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CheckOutBranch)
            .WithMany()
            .HasForeignKey(a => a.CheckOutBranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- ÍNDICES OPTIMIZADOS (Poder Relacional) ---
        // 1. Restricción única: Un registro por empleado al día 🔐
        builder.HasIndex(a => new { a.EmployeeId, a.Date }).IsUnique();

        // 2. Índice compuesto para búsquedas de historial de un empleado 📅
        // (Ya cubre la necesidad de indexar a.EmployeeId solo)
        builder.HasIndex(a => new { a.EmployeeId, a.CheckIn });

        // 3. Índice compuesto para ver asistencias por Sede en fechas específicas 🏬
        builder.HasIndex(a => new { a.CheckInBranchId, a.Date });
        builder.HasIndex(a => new { a.CheckOutBranchId, a.Date });
    }
}
