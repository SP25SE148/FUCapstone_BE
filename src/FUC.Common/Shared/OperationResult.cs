namespace FUC.Common.Shared;
public class OperationResult
{
    protected internal OperationResult(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static OperationResult Success() => new(true, Error.None);

    public static OperationResult<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

    public static OperationResult Failure(Error error) =>
        new(false, error);

    public static OperationResult<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);

    public static OperationResult<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}
