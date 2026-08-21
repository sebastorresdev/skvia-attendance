using System.Text.RegularExpressions;

namespace Skvia.Erp.Domain.Employees;

public readonly partial record struct Phone
{
    // A simple regex for phone numbers, allowing digits, spaces, hyphens, and plus signs.
    [GeneratedRegex(@"^\+?[\d\s-]{7,20}$")]
    private static partial Regex PhoneRegex();

    public string Value { get; }

    private Phone(string value)
    {
        Value = value;
    }

    public static Phone Create(string phoneNumber)
    {
        ArgumentNullException.ThrowIfNull(phoneNumber);
        var trimmed = phoneNumber.Trim();
        ArgumentOutOfRangeException.ThrowIfGreaterThan(trimmed.Length, EmployeeConstants.PhoneMaxLength);

        if (!PhoneRegex().IsMatch(trimmed))
        {
            throw new ArgumentException("Invalid phone number format.", nameof(phoneNumber));
        }

        return new Phone(trimmed);
    }

    public override string ToString() => Value;
}

