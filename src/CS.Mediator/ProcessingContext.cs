using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CS.Mediator.Contract;

namespace CS.Mediator;

public class ProcessingContext
{
    private readonly ConcurrentDictionary<string, object> payloads = new();

    public StatusCode StatusCode { get; private set; } = StatusCode.Ok;

    public bool TryAddPayload(string key, object payload)
    {
        return this.payloads.TryAdd(key, payload);
    }

    public object? TryGetPayload(string key)
    {
        return this.payloads.TryGetValue(key, out var payload) ? payload : null;
    }

    public void WriteTo(StatusCode statusCode)
    {
        this.StatusCode = statusCode;
    }
}

public class ProcessingContext <TRequest, TResult> : ProcessingContext
    where TRequest : IRequest
    where TResult : class?
{
    private readonly List<ValidationResult> validationResults = new();

    internal ProcessingContext(TRequest request, CancellationToken token)
    {
        this.Request = request;
        this.Token = token;
    }

    public bool IsValid => !this.validationResults.Any();

    public TRequest Request { get; }
    internal TResult Result { get; private set; } = default!;

    public CancellationToken Token { get; }

    public void WriteTo(IEnumerable<ValidationResult> results)
    {
        this.validationResults.AddRange(results);
    }

    internal void WriteTo(TResult result)
    {
        this.Result = result;
    }

    internal RequestResult<TResult> ToRequestResult()
    {
        if (this.StatusCode != StatusCode.Ok || !this.IsValid)
        {
            return new RequestResult<TResult>(this.validationResults, this.StatusCode);
        }

        return new RequestResult<TResult>(this.Result, this.StatusCode);
    }
}