using System.Collections.Generic;
using System.Linq;

namespace Mediator.Contract;

public record RequestResult<TResult>
    where TResult : class?
{
    internal RequestResult(TResult result, int statusCode)
    {
        this.Result = result;
        this.StatusCode = statusCode;
    }

    internal RequestResult(IEnumerable<ProcessingResult> validationResults, int statusCode)
    {
        this.ProcessingResults = new ProcessingResults(validationResults);
        this.StatusCode = statusCode;
    }

    public TResult Result { get; } = default!;
    public int StatusCode { get; }
    public bool IsSuccessful => this.StatusCode == StatusCodes.Ok && !this.ProcessingResults.Any();
    public ProcessingResults ProcessingResults { get; } = ProcessingResults.Empty;
}