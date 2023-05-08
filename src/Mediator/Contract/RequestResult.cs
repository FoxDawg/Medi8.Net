using System.Collections.Generic;
using System.Linq;

namespace Mediator.Contract;

public record RequestResult : RequestResult<NoResult>
{
    internal RequestResult(Status status)
        : base(NoResult.Create(), status)
    {
    }

    internal RequestResult(IEnumerable<Error> errors, Status status)
        : base(errors, status)
    {
    }

    internal static RequestResult FromResult<T>(RequestResult<T> result)
        where T : class
    {
        if (result.IsSuccessful)
        {
            return new RequestResult(result.Status);
        }

        return new RequestResult(result.Errors.ToList(), result.Status);
    }
}

public record RequestResult<TResult>
    where TResult : class?
{
    internal RequestResult(TResult result, Status status)
    {
        this.Result = result;
        this.Status = status;
    }

    internal RequestResult(IEnumerable<Error> errors, Status status)
    {
        this.Errors = new Errors(errors);
        this.Status = status;
    }

    public TResult Result { get; } = default!;
    public Status Status { get; }
    public bool IsSuccessful => this.Status == Status.Ok && !this.Errors.Any();
    public Errors Errors { get; } = Errors.Empty;
}