using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mediator.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

public class ProcessingContext
{
    private readonly ConcurrentDictionary<string, object> payloads = new();
    private readonly List<ProcessingResult> processingResults = new();
    public bool IsValid => !this.processingResults.Any();
    protected ProcessingResults ProcessingResults => new(this.processingResults);
    public int StatusCode { get; private set; } = StatusCodes.Ok;

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

    public void WriteTo(int statusCode)
    {
        this.StatusCode = statusCode;
    }
}

public class ProcessingContext <TRequest> : ProcessingContext
{
    private readonly IServiceScope scope;

    public ProcessingContext(IServiceScope scope)
    {
        this.scope = scope;
    }

    public TRequest Request { get; protected init; } = default!;

    public object? GetService(Type type)
    {
        return this.scope.ServiceProvider.GetService(type);
    }
    
    public T GetRequiredService <T>()
        where T : notnull
    {
        return this.scope.ServiceProvider.GetRequiredService<T>();
    }
}

public class ProcessingContext <TRequest, TResult> : ProcessingContext<TRequest>
    where TRequest : IRequest
    where TResult : class?
{
    internal ProcessingContext(IServiceScope scope, TRequest request, CancellationToken token)
        : base(scope)
    {
        this.Request = request;
        this.Token = token;
    }

    internal TResult Result { get; private set; } = default!;

    public CancellationToken Token { get; }

    internal void WriteTo(TResult result)
    {
        this.Result = result;
    }

    internal RequestResult<TResult> ToRequestResult()
    {
        if (this.StatusCode != StatusCodes.Ok || !this.IsValid)
        {
            return new RequestResult<TResult>(this.ProcessingResults, this.StatusCode);
        }

        return new RequestResult<TResult>(this.Result, this.StatusCode);
    }
}