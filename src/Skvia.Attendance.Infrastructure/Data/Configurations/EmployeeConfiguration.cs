using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        builder.HasOne<Skvia.Attendance.Domain.Branches.Branch>()
            .WithMany()
            .HasForeignKey(e => e.MainBranchId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure DocumentIdentifier as an owned entity
        builder.OwnsOne(p => p.DocumentIdentifier, navigationBuilder =>
        {
            navigationBuilder.Property(di => di.Type)
                .HasColumnName("DocumentType")
                .IsRequired()
                .HasConversion<int>(); // Store enum as int

            navigationBuilder.Property(di => di.Number)
                .HasColumnName("DocumentNumber")
                .IsRequired()
                .HasMaxLength(EmployeeConstants.DocumentNumberMaxLength);

            navigationBuilder.HasIndex(di => new { di.Type, di.Number }).IsUnique();
        });

        // Configure Email with a ValueConverter
        builder.Property(p => p.Email)
            .HasConversion(
                v => v.HasValue ? v.Value.Value : null, // Convert Email? to string?
                v => v != null ? Email.Create(v) : (Email?)null) // Convert string? to Email?
            .HasMaxLength(EmployeeConstants.EmailMaxLength)
            .IsRequired(false);

        // Configure Phone with a ValueConverter
        builder.Property(p => p.Phone)
            .HasConversion(
                v => v.HasValue ? v.Value.Value : null, // Convert Phone? to string?
                v => v != null ? Phone.Create(v) : (Phone?)null) // Convert string? to Phone?
            .HasMaxLength(EmployeeConstants.PhoneMaxLength)
            .IsRequired(false);

        builder.Property(p => p.Position).IsRequired(false).HasMaxLength(EmployeeConstants.PositionMaxLength);
        builder.Property(p => p.Department).IsRequired(false).HasMaxLength(EmployeeConstants.DepartmentMaxLength);
        builder.Property(p => p.HireDate).IsRequired();
        builder.Property(p => p.PhotoUrl).IsRequired(false).HasMaxLength(EmployeeConstants.PhotoUrlMaxLength);
    }
}
