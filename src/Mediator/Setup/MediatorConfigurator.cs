using System;
using System.Collections.Generic;
using System.Linq;
using Mediator.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.Setup;

public class MediatorConfigurator
{
    private readonly HashSet<HandlerMap> handlers;
    private readonly List<IProcessor> postprocessors;
    private readonly List<IProcessor> preprocessors;
    private readonly IServiceCollection serviceCollection;

    public MediatorConfigurator(IServiceCollection serviceCollection)
    {
        this.serviceCollection = serviceCollection;

        this.handlers = new HashSet<HandlerMap>();
        this.preprocessors = new List<IProcessor>();
        this.postprocessors = new List<IProcessor>();
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

    public void AddPreExecutionMiddleware<TMiddleware>()
        where TMiddleware : IPreProcessor, new()
    {
        this.preprocessors.Add(new TMiddleware());
    }

    public void AddPostExecutionMiddleware<TMiddleware>()
        where TMiddleware : IPostProcessor, new()
    {
        this.postprocessors.Add(new TMiddleware());
    }
}

internal record HandlerMap(Type RequestType, Type HandlerType);