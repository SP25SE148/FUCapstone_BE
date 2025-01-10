namespace FUC.Common.Shared;

public sealed class ValidationResult : OperationResult, IValidationResult
{
    private ValidationResult(Error[] errors)
        : base(false, IValidationResult.ValidationError) =>
        Errors = errors;

    public Error[] Errors { get; }

    public static ValidationResult WithErrors(params Error[] errors) => new(errors);
}
