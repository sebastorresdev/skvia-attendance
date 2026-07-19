using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Code).IsRequired().HasMaxLength(EmployeeConstants.CodeMaxLength);
        builder.HasIndex(p => p.Code).IsUnique();
        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(EmployeeConstants.FirstNameMaxLength);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(EmployeeConstants.LastNameMaxLength);
        builder.Property(p => p.DocumentType).IsRequired().HasConversion<int>();
        builder.Property(p => p.DocumentNumber).IsRequired().HasMaxLength(EmployeeConstants.DocumentNumberMaxLength);
        builder.HasIndex(p => p.DocumentNumber).IsUnique(); // Index
        builder.Property(p => p.Email).IsRequired(false).HasMaxLength(EmployeeConstants.EmailMaxLength);
        builder.Property(p => p.Phone).IsRequired(false).HasMaxLength(EmployeeConstants.PhoneMaxLength);
        builder.Property(p => p.Position).IsRequired(false).HasMaxLength(EmployeeConstants.PositionMaxLength);
        builder.Property(p => p.Department).IsRequired(false).HasMaxLength(EmployeeConstants.DepartmentMaxLength);
        builder.Property(p => p.HireDate).IsRequired();
        builder.Property(p => p.PhotoUrl).IsRequired(false).HasMaxLength(EmployeeConstants.PhotoUrlMaxLength);
    }
}
