using Skvia.Erp.Domain.Common;

namespace Skvia.Erp.Domain.Workplaces;

public class Workplace : BaseAuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Address { get; private set; }
    public string TimeZoneId { get; private set; } = "America/Lima";
    
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public double GeofenceRadiusMeters { get; private set; }
    public bool RequirePhotoForMobile { get; private set; } = true;

    private Workplace() { } // EF Core

    public static Workplace Create(
        string code,
        string name,
        string timeZoneId,
        double? latitude,
        double? longitude,
        double geofenceRadiusMeters,
        string? address = null,
        bool requirePhotoForMobile = true)
    {
        var workplace = new Workplace();

        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(name);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(code.Length, WorkplaceConstants.CodeMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(name.Length, WorkplaceConstants.NameMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(address?.Length ?? 0, WorkplaceConstants.AddressMaxLength);
        ArgumentException.ThrowIfNullOrWhiteSpace(timeZoneId);
        if (geofenceRadiusMeters <= 0) throw new ArgumentOutOfRangeException(nameof(geofenceRadiusMeters), "Radio debe ser mayor a 0.");

        workplace.Code = code.Trim().ToUpper();
        workplace.Name = name.Trim();
        workplace.TimeZoneId = timeZoneId.Trim();
        workplace.Address = address?.Trim();
        workplace.Latitude = latitude;
        workplace.Longitude = longitude;
        workplace.GeofenceRadiusMeters = geofenceRadiusMeters;
        workplace.RequirePhotoForMobile = requirePhotoForMobile;

        return workplace;
    }

    public void Update(
        string code,
        string name,
        string? timeZoneId,
        double? latitude,
        double? longitude,
        double geofenceRadiusMeters,
        string? address = null,
        bool? requirePhotoForMobile = null)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(name);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(code.Length, WorkplaceConstants.CodeMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(name.Length, WorkplaceConstants.NameMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(address?.Length ?? 0, WorkplaceConstants.AddressMaxLength);
        if (geofenceRadiusMeters <= 0) throw new ArgumentOutOfRangeException(nameof(geofenceRadiusMeters), "Radio debe ser mayor a 0.");

        Code = code.Trim().ToUpper();
        Name = name.Trim();
        Address = address?.Trim();
        if (!string.IsNullOrWhiteSpace(timeZoneId)) TimeZoneId = timeZoneId.Trim();
        Latitude = latitude;
        Longitude = longitude;
        GeofenceRadiusMeters = geofenceRadiusMeters;
        if (requirePhotoForMobile.HasValue) RequirePhotoForMobile = requirePhotoForMobile.Value;
    }
}

