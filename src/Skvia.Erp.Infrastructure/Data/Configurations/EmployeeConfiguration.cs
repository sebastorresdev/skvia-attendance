using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Skvia.Erp.Domain.Employees;

namespace Skvia.Erp.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Code).IsRequired().HasMaxLength(EmployeeConstants.CodeMaxLength);
        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.DepartmentId);
        builder.HasIndex(p => p.MainBranchId);
        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(EmployeeConstants.FirstNameMaxLength);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(EmployeeConstants.LastNameMaxLength);

        builder.HasOne<Skvia.Erp.Domain.Branches.Branch>()
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
        
        builder.HasOne<Skvia.Erp.Domain.Departments.Department>()
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(p => p.HireDate).IsRequired();
        builder.Property(p => p.PhotoUrl).IsRequired(false).HasMaxLength(EmployeeConstants.PhotoUrlMaxLength);
        
        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(EmployeeStatus.Active);
            
        builder.Property(p => p.ApplicationUserId).IsRequired(false).HasMaxLength(450);
        builder.HasIndex(p => p.ApplicationUserId).IsUnique();
        builder.Property(p => p.MobileCheckInEnabled).IsRequired().HasDefaultValue(false);
        builder.Property(p => p.RequireFourPointAttendance).IsRequired().HasDefaultValue(false);
        builder.Property(p => p.IsAttendanceTracked).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.AutoCompleteClockOut).IsRequired().HasDefaultValue(false);

        builder.Property(p => p.TardinessToleranceMinutes).IsRequired().HasDefaultValue(0);

        builder.Property(p => p.AllowedWorkplaceIds)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v) || !v.Contains("[") ? new List<Guid>() : System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<Guid>())
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[]'::jsonb");
    }
}

