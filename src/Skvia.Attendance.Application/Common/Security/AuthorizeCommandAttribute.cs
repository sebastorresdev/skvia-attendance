namespace Skvia.Attendance.Application.Common.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AuthorizeCommandAttribute : Attribute
{
    public string Permissions { get; set; } = string.Empty;
    public string Roles { get; set; } = string.Empty;
}
