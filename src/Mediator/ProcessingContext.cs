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
    private readonly List<Error> errors = new();
    private readonly ConcurrentDictionary<string, object> payloads = new();
    public bool IsValid => !this.errors.Any();
    protected Errors Errors => new(this.errors);
    public Status Status { get; private set; } = Status.Ok;

    public bool TryAddPayload(string key, object payload)
    {
        return this.payloads.TryAdd(key, payload);
    }

    public void WriteTo(IEnumerable<Error> results)
    {
        this.errors.AddRange(results);
    }

    public void WriteTo(Error result)
    {
        this.errors.Add(result);
    }

    public object? TryGetPayload(string key)
    {
        return this.payloads.TryGetValue(key, out var payload) ? payload : null;
    }

    public void WriteTo(Status status)
    {
        this.Status = status;
    }
}

public class ProcessingContext<TRequest> : ProcessingContext, IProcessingContext<TRequest>
    where TRequest : IRequest
{
    private readonly IServiceScope scope;

    public ProcessingContext(IServiceScope scope, CancellationToken token)
    {
        this.Token = token;
        this.scope = scope;
    }

    public CancellationToken Token { get; }
    public TRequest Request { get; protected init; } = default!;

    public object? GetService(Type type)
    {
        return this.scope.ServiceProvider.GetService(type);
    }

    public T GetRequiredService<T>()
        where T : notnull
    {
        return this.scope.ServiceProvider.GetRequiredService<T>();
    }
}

public class ProcessingContext<TRequest, TResult> : ProcessingContext<TRequest>
    where TRequest : IRequest
    where TResult : class?
{
    internal ProcessingContext(IServiceScope scope, TRequest request, CancellationToken token)
        : base(scope, token)
    {
        this.Request = request;
    }

    internal TResult Result { get; private set; } = default!;

    internal void WriteTo(TResult result)
    {
        this.Result = result;
    }

    internal RequestResult<TResult> ToRequestResult()
    {
        if (this.Status != Status.Ok || !this.IsValid)
        {
            return new RequestResult<TResult>(this.Errors, this.Status);
        }

        return new RequestResult<TResult>(this.Result, this.Status);
    }
}