namespace Skvia.Erp.Domain.Employees;

public record DocumentIdentifier
{
    public DocumentType Type { get; init; }
    public string Number { get; init; } = null!;

    private DocumentIdentifier() { }

    private DocumentIdentifier(DocumentType type, string number)
    {
        Type = type;
        Number = number;
    }

    public static DocumentIdentifier Create(DocumentType type, string number)
    {
        ArgumentNullException.ThrowIfNull(number);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(number.Length, EmployeeConstants.DocumentNumberMaxLength);

        return new DocumentIdentifier(type, number.Trim());
    }

    public override string ToString() => $"{Type}: {Number}";
}

