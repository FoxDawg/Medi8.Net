using System;
using System.Collections.Generic;
using System.Linq;
using CS.Mediator.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace CS.Mediator.Setup;

public class MediatorConfigurator
{
    private readonly HashSet<HandlerMap> handlers;
    private readonly List<Func<IServiceProvider, IPipelineFilter>> pipelineFilters;
    private readonly IServiceCollection serviceCollection;

    public MediatorConfigurator(IServiceCollection serviceCollection)
    {
        this.serviceCollection = serviceCollection;

        this.handlers = new HashSet<HandlerMap>();
        this.pipelineFilters = new List<Func<IServiceProvider, IPipelineFilter>>();
    }

    internal MediatorConfiguration Build()
    {
        return new MediatorConfiguration(this.handlers, this.pipelineFilters);
    }

    public void AddHandler<TRequest, THandler>()
        where THandler : class
    {
        if (this.handlers.Any(o => o.RequestType == typeof(TRequest)))
        {
            throw new InvalidOperationException($"A handler for type {typeof(TRequest)} is already registered.");
        }

        this.serviceCollection.AddScoped<THandler>();
        this.handlers.Add(new HandlerMap(typeof(TRequest), typeof(THandler)));
    }

    public void AddPipelineFilter<TFilter>(Func<IServiceProvider, TFilter> factoryFunc)
        where TFilter : class, IPipelineFilter
    {
        this.serviceCollection.AddScoped(factoryFunc);
        this.pipelineFilters.Add(factoryFunc);
    }
}

internal record HandlerMap(Type RequestType, Type HandlerType);