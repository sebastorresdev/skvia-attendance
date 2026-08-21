using System.Text.RegularExpressions;

namespace Skvia.Erp.Domain.Employees;

public readonly partial record struct Email
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string emailAddress)
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        var trimmed = emailAddress.Trim();
        ArgumentOutOfRangeException.ThrowIfGreaterThan(trimmed.Length, EmployeeConstants.EmailMaxLength);

        if (!EmailRegex().IsMatch(trimmed))
        {
            throw new ArgumentException("Invalid email address format.", nameof(emailAddress));
        }

        return new Email(trimmed);
    }

    public override string ToString() => Value;
}

