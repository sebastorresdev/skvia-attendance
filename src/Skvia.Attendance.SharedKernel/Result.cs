using System.Diagnostics.CodeAnalysis;

namespace Skvia.Attendance.SharedKernel;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result FromException(Exception ex)
        => Failure(Error.Failure("General.Exception", ex.Message));
}


public class Result<TValue>(TValue? value, bool isSuccess, Error error) : Result(isSuccess, error)
{
    [NotNull]
    public TValue Value
    {
        get => IsSuccess
        ? field!
        : throw new InvalidOperationException("Cannot access value of a failure result.");
    } = value;

    public static Result<TValue> Success(TValue value)
        => new(value, true, Error.None);

    public static new Result<TValue> Failure(Error error)
        => new(default, false, error);

    public static new Result<TValue> FromException(Exception ex)
        => Failure(Error.Failure("General.Exception", ex.Message));

    public static implicit operator Result<TValue>(TValue? value)
        => value is not null ? Success(value) : Failure(Error.NullValue);
}
