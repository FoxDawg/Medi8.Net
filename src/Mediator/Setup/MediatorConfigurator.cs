using System;
using System.Collections.Generic;
using System.Linq;
using Mediator.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.Setup;

public class MediatorConfigurator
{
    private readonly HashSet<HandlerMap> handlers;
    private readonly List<Func<IServiceProvider, IProcessor>> postprocessors;
    private readonly List<Func<IServiceProvider, IProcessor>> preprocessors;
    private readonly IServiceCollection serviceCollection;

    public MediatorConfigurator(IServiceCollection serviceCollection)
    {
        this.serviceCollection = serviceCollection;

        this.handlers = new HashSet<HandlerMap>();
        this.preprocessors = new List<Func<IServiceProvider, IProcessor>>();
        this.postprocessors = new List<Func<IServiceProvider, IProcessor>>();
    }

    internal MediatorConfiguration Build()
    {
        return new MediatorConfiguration(this.handlers, this.preprocessors, this.postprocessors);
    }

    public void AddHandler <TRequest, THandler>()
        where THandler : class
    {
        if (this.handlers.Any(o => o.RequestType == typeof(TRequest)))
        {
            throw new InvalidOperationException($"A handler for type {typeof(TRequest)} is already registered.");
        }

        this.serviceCollection.AddScoped<THandler>();
        this.handlers.Add(new HandlerMap(typeof(TRequest), typeof(THandler)));
    }

    public void AddToPipeline(Func<IServiceProvider, IPreProcessor> factoryFunc)
    {
        this.serviceCollection.AddScoped(factoryFunc);
        this.preprocessors.Add(factoryFunc);
    }

    public void AddToPipeline(Func<IServiceProvider, IPostProcessor> factoryFunc)
    {
        this.serviceCollection.AddScoped(factoryFunc);
        this.postprocessors.Add(factoryFunc);
    }
}

internal record HandlerMap(Type RequestType, Type HandlerType);