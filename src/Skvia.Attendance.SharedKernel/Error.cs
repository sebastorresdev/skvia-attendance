namespace Skvia.Attendance.SharedKernel;

public record Error
{
    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }

    protected Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
    public static readonly Error NullValue = new("General.Null", "Null value was provided", ErrorType.Validation);

    public static Error NotFound(string code, string description)
        => new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description)
        => new(code, description, ErrorType.Conflict);

    public static Error Unauthorized(string code, string description)
        => new(code, description, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string description)
        => new(code, description, ErrorType.Forbidden);

    public static Error Failure(string code, string description)
        => new(code, description, ErrorType.Failure);
}
