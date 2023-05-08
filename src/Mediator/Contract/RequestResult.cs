using System.Collections.Generic;
using System.Linq;

namespace Mediator.Contract;

public record RequestResult : RequestResult<NoResult>
{
    internal RequestResult(int statusCode)
        : base(NoResult.Create(), statusCode)
    {
    }

    internal RequestResult(IEnumerable<Error> errors, int statusCode)
        : base(errors, statusCode)
    {
    }

    internal static RequestResult FromResult<T>(RequestResult<T> result)
        where T : class
    {
        if (result.IsSuccessful)
        {
            return new RequestResult(result.StatusCode);
        }

        return new RequestResult(result.Errors.ToList(), result.StatusCode);
    }
}

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