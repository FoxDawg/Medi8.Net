using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mediator.Contract;

namespace Mediator;

public class ProcessingContext
{
    private readonly ConcurrentDictionary<string, object> payloads = new();
    private readonly List<ProcessingResult> processingResults = new();
    public bool IsValid => !this.processingResults.Any();
    protected ProcessingResults ProcessingResults => new(this.processingResults);
    public StatusCode StatusCode { get; private set; } = StatusCode.Ok;

    public bool TryAddPayload(string key, object payload)
    {
        return this.payloads.TryAdd(key, payload);
    }

    public void WriteTo(IEnumerable<ProcessingResult> results)
    {
        this.processingResults.AddRange(results);
    }

    public void WriteTo(ProcessingResult result)
    {
        this.processingResults.Add(result);
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
    internal ProcessingContext(TRequest request, CancellationToken token)
    {
        this.Request = request;
        this.Token = token;
    }

    public TRequest Request { get; }
    internal TResult Result { get; private set; } = default!;

    public CancellationToken Token { get; }

    internal void WriteTo(TResult result)
    {
        this.Result = result;
    }

    internal RequestResult<TResult> ToRequestResult()
    {
        if (this.StatusCode != StatusCode.Ok || !this.IsValid)
        {
            return new RequestResult<TResult>(this.ProcessingResults, this.StatusCode);
        }

        return new RequestResult<TResult>(this.Result, this.StatusCode);
    }
}