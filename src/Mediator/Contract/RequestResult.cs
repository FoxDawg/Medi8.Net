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

    internal RequestResult(IEnumerable<Error> errors, int statusCode)
    {
        this.Errors = new Errors(errors);
        this.StatusCode = statusCode;
    }

    public TResult Result { get; } = default!;
    public int StatusCode { get; }
    public bool IsSuccessful => this.StatusCode == StatusCodes.Ok && !this.Errors.Any();
    public Errors Errors { get; } = Errors.Empty;
}