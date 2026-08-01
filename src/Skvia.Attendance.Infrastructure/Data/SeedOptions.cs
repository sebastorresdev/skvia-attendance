namespace Skvia.Attendance.Infrastructure.Data;

public class SeedOptions
{
    public const string SectionName = "Seed";

    public bool Enabled { get; set; }

    public string? AdminUserName { get; set; }

    public string? AdminEmail { get; set; }

    public string? AdminPassword { get; set; }
}
