using System;
using System.Collections.Generic;
using System.Linq;
using Mediator.Contract;

namespace Mediator.Setup;

internal class MediatorConfiguration
{
    private readonly HashSet<HandlerMap> handlers;
    private readonly List<IProcessor> postProcessors;
    private readonly List<IProcessor> preProcessors;

    public MediatorConfiguration(HashSet<HandlerMap> handlers, List<IProcessor> preProcessors, List<IProcessor> postProcessors)
    {
        this.handlers = handlers;
        this.preProcessors = preProcessors;
        this.postProcessors = postProcessors;
    }

    public IList<IProcessor> Preprocessors => this.preProcessors;
    public IList<IProcessor> Postprocessors => this.postProcessors;

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