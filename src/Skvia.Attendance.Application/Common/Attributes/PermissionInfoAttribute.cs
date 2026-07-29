namespace Skvia.Attendance.Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class PermissionInfoAttribute(string display, string description) : Attribute
{
    public string Display { get; } = display;
    public string Description { get; } = description;
}
