using CS.Mediator.Contract;

namespace CS.Mediator;

public class ProcessingContext
{
    public StatusCode StatusCode { get; private set; } = StatusCode.Ok;

    public void WriteTo(StatusCode statusCode)
    {
        StatusCode = statusCode;
    }
}

public class ProcessingContext<TRequest, TResult> : ProcessingContext
    where TRequest : IRequest
    where TResult : class?
{
    private readonly List<ValidationResult> _validationResults = new();

    internal ProcessingContext(TRequest request, CancellationToken token)
    {
        Request = request;
        Token = token;
    }

    public bool IsValid => !_validationResults.Any();

    public TRequest Request { get; }
    internal TResult Result { get; private set; } = default!;

    public CancellationToken Token { get; }

    public void WriteTo(IEnumerable<ValidationResult> validationResults)
    {
        _validationResults.AddRange(validationResults);
    }

    internal void WriteTo(TResult result)
    {
        Result = result;
    }

    internal RequestResult<TResult> ToRequestResult()
    {
        if (StatusCode != StatusCode.Ok || !IsValid)
        {
            return new RequestResult<TResult>(_validationResults, StatusCode);
        }

        return new RequestResult<TResult>(Result, StatusCode);
    }
}