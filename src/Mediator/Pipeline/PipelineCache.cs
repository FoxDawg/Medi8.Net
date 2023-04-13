using System;
using System.Collections.Concurrent;
using Mediator.Contract;

namespace Mediator.Pipeline;

internal class PipelineCache
{
    private readonly ConcurrentDictionary<Type, object> postprocessors = new();
    private readonly ConcurrentDictionary<Type, object> preprocessors = new();

    public Next<TRequest> GetOrAddPreprocessor<TRequest>(Func<Next<TRequest>> factoryFunc)
        where TRequest : IRequest
    {
        var pipeline = this.preprocessors.GetOrAdd(typeof(TRequest), _ => factoryFunc());
        return (Next<TRequest>)pipeline;
    }

    public Next<TRequest> GetOrAddPostprocessor<TRequest>(Func<Next<TRequest>> factoryFunc)
        where TRequest : IRequest
    {
        var pipeline = this.postprocessors.GetOrAdd(typeof(TRequest), _ => factoryFunc());
        return (Next<TRequest>)pipeline;
    }
}