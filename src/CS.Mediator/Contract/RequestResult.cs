namespace CS.Mediator.Contract;

public record RequestResult<TResult>
    where TResult : class?
{
    internal RequestResult(TResult result, StatusCode statusCode)
    {
        Result = result;
        StatusCode = statusCode;
    }

    internal RequestResult(IEnumerable<ValidationResult> validationResults, StatusCode statusCode)
    {
        ValidationResults = new ValidationResults(validationResults);
        StatusCode = statusCode;
    }

    public TResult Result { get; } = default!;
    public StatusCode StatusCode { get; }
    public bool IsSuccessful => StatusCode == StatusCode.Ok && !ValidationResults.Any();
    public ValidationResults ValidationResults { get; } = ValidationResults.Empty;
}