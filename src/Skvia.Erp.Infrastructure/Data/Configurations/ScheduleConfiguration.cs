using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Domain.Schedules;

namespace Skvia.Erp.Infrastructure.Data.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.ToTable("schedules");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.Code).IsRequired().HasMaxLength(20);
        builder.Property(s => s.Description).IsRequired().HasMaxLength(150);
        builder.Property(s => s.TimeZoneId).IsRequired().HasMaxLength(100);
        
        builder.Property(s => s.DefaultStartTime).IsRequired();
        builder.Property(s => s.DefaultEndTime).IsRequired();
        
        builder.Property(s => s.HasBreak).IsRequired().HasDefaultValue(false);
        builder.Property(s => s.BreakStartTime).IsRequired(false);
        builder.Property(s => s.BreakEndTime).IsRequired(false);
    }
}


