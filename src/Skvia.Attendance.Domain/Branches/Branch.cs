using Skvia.Attendance.Domain.Common;

namespace Skvia.Attendance.Domain.Branches;

public class Branch : BaseAuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Address { get; private set; }
    public string TimeZoneId { get; private set; } = "America/Lima";
    public int TardinessToleranceMinutes { get; private set; } = 0;

    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public double? GeofenceRadiusMeters { get; private set; }
    public bool RequirePhotoForMobile { get; private set; } = true;

    private readonly List<BranchUser> _branchUsers = [];
    public IReadOnlyCollection<BranchUser> BranchUsers => _branchUsers.AsReadOnly();

    private Branch() { } // EF Core

    public static Branch Create(
        string code,
        string name,
        string? address = null,
        string timeZoneId = "America/Lima",
        int tardinessToleranceMinutes = 0,
        double? latitude = null,
        double? longitude = null,
        double? geofenceRadiusMeters = null,
        bool requirePhotoForMobile = true)
    {
        var branch = new Branch();

        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(timeZoneId);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(code.Length, BranchConstants.CodeMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(name.Length, BranchConstants.NameMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(address?.Length ?? 0, BranchConstants.AddressMaxLength);

        branch.Code = code.Trim().ToUpper();
        branch.Name = name.Trim();
        branch.Address = address?.Trim();
        branch.TimeZoneId = timeZoneId.Trim();
        branch.TardinessToleranceMinutes = tardinessToleranceMinutes >= 0 ? tardinessToleranceMinutes : 0;
        branch.Latitude = latitude;
        branch.Longitude = longitude;
        branch.GeofenceRadiusMeters = geofenceRadiusMeters;
        branch.RequirePhotoForMobile = requirePhotoForMobile;

        return branch;
    }

    public void Update(
        string code,
        string name,
        string? address = null,
        string? timeZoneId = null,
        int? tardinessToleranceMinutes = null,
        double? latitude = null,
        double? longitude = null,
        double? geofenceRadiusMeters = null,
        bool? requirePhotoForMobile = null)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(name);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(code.Length, BranchConstants.CodeMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(name.Length, BranchConstants.NameMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(address?.Length ?? 0, BranchConstants.AddressMaxLength);

        Code = code.Trim().ToUpper();
        Name = name.Trim();
        Address = address?.Trim();
        if (!string.IsNullOrWhiteSpace(timeZoneId))
            TimeZoneId = timeZoneId.Trim();
        if (tardinessToleranceMinutes.HasValue && tardinessToleranceMinutes.Value >= 0)
            TardinessToleranceMinutes = tardinessToleranceMinutes.Value;

        if (latitude.HasValue) Latitude = latitude;
        if (longitude.HasValue) Longitude = longitude;
        if (geofenceRadiusMeters.HasValue) GeofenceRadiusMeters = geofenceRadiusMeters;
        if (requirePhotoForMobile.HasValue) RequirePhotoForMobile = requirePhotoForMobile.Value;
    }
}
