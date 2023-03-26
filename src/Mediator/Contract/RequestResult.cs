using System.Collections.Generic;
using System.Linq;

namespace Mediator.Contract;

public record RequestResult<TResult>
    where TResult : class?
{
    internal RequestResult(TResult result, StatusCode statusCode)
    {
        this.Result = result;
        this.StatusCode = statusCode;
    }

    internal RequestResult(IEnumerable<ProcessingResult> validationResults, StatusCode statusCode)
    {
        this.ProcessingResults = new ProcessingResults(validationResults);
        this.StatusCode = statusCode;
    }

    public TResult Result { get; } = default!;
    public StatusCode StatusCode { get; }
    public bool IsSuccessful => this.StatusCode == StatusCode.Ok && !this.ProcessingResults.Any();
    public ProcessingResults ProcessingResults { get; } = ProcessingResults.Empty;
}