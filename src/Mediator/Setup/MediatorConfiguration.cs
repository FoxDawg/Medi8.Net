using System;
using System.Collections.Generic;
using System.Linq;
using Mediator.Contract;

namespace Mediator.Setup;

internal class MediatorConfiguration
{
    private readonly HashSet<HandlerMap> handlers;
    private readonly List<Func<IServiceProvider, IProcessor>> preProcessors;
    private readonly List<Func<IServiceProvider, IProcessor>> postProcessors;

    public MediatorConfiguration(HashSet<HandlerMap> handlers, List<Func<IServiceProvider, IProcessor>> preProcessors, List<Func<IServiceProvider, IProcessor>> postProcessors)
    {
        this.handlers = handlers;
        this.preProcessors = preProcessors;
        this.postProcessors = postProcessors;
    }

    public IList<Func<IServiceProvider, IProcessor>> Preprocessors => this.preProcessors;
    public IList<Func<IServiceProvider, IProcessor>> Postprocessors => this.postProcessors;

    public Type GetHandler<TRequest>()
    {
        var mapping = this.handlers.FirstOrDefault(o => o.RequestType == typeof(TRequest));
        if (mapping is not null)
        {
            return mapping.HandlerType;
        }

        throw new InvalidOperationException($"No handler registered for type {typeof(TRequest)}");
    }
}