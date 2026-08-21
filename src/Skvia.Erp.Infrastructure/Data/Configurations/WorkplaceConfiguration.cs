using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skvia.Erp.Domain.Workplaces;

namespace Skvia.Erp.Infrastructure.Data.Configurations;

public class WorkplaceConfiguration : IEntityTypeConfiguration<Workplace>
{
    public void Configure(EntityTypeBuilder<Workplace> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(WorkplaceConstants.CodeMaxLength)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(WorkplaceConstants.NameMaxLength)
            .IsRequired();

        builder.Property(x => x.Address)
            .HasMaxLength(WorkplaceConstants.AddressMaxLength)
            .IsRequired(false);

        builder.Property(x => x.TimeZoneId)
            .IsRequired();

        builder.Property(x => x.Latitude)
            .IsRequired();

        builder.Property(x => x.Longitude)
            .IsRequired();

        builder.Property(x => x.GeofenceRadiusMeters)
            .IsRequired();

        builder.Property(x => x.RequirePhotoForMobile)
            .IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();
    }
}

