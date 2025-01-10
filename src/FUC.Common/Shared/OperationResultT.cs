namespace FUC.Common.Shared;

public class OperationResult<TValue> : OperationResult
{
    private readonly TValue? _value;

    protected internal OperationResult(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error) =>
        _value = value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static implicit operator OperationResult<TValue>(TValue? value) => Create(value);
}
