using System;
using System.Collections.Generic;
using System.Linq;
using CS.Mediator.Contract;

namespace CS.Mediator.Setup;

internal class MediatorConfiguration
{
    private readonly HashSet<HandlerMap> handlers;
    private readonly List<Func<IServiceProvider, IPipelineFilter>> pipelineFilters;

    public MediatorConfiguration(HashSet<HandlerMap> handlers, List<Func<IServiceProvider, IPipelineFilter>> pipelineFilters)
    {
        this.handlers = handlers;
        this.pipelineFilters = pipelineFilters;
    }

    public IList<Func<IServiceProvider, IPipelineFilter>> GetFilters()
    {
        return this.pipelineFilters;
    }

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