using CS.Mediator.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace CS.Mediator.Setup;

public class MediatorConfigurator
{
    private readonly HashSet<HandlerMap> _handlers;
    private readonly IServiceCollection _serviceCollection;
    private readonly List<Func<IServiceProvider, IPipelineFilter>> _pipelineFilters;

    public MediatorConfigurator(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;

        _handlers = new HashSet<HandlerMap>();
        _pipelineFilters = new List<Func<IServiceProvider, IPipelineFilter>>();
    }

    internal MediatorConfiguration Build()
    {
        return new MediatorConfiguration(_handlers, _pipelineFilters);
    }

    public void AddHandler<TRequest, THandler>()
        where THandler : class
    {
        if (_handlers.Any(o => o.RequestType == typeof(TRequest)))
        {
            throw new InvalidOperationException($"A handler for type {typeof(TRequest)} is already registered.");
        }

        _serviceCollection.AddScoped<THandler>();
        _handlers.Add(new HandlerMap(typeof(TRequest), typeof(THandler)));
    }

    public void AddPipelineFilter<TFilter>(Func<IServiceProvider, TFilter> factoryFunc)
        where TFilter : class, IPipelineFilter
    {
        _serviceCollection.AddScoped(factoryFunc);
        _pipelineFilters.Add(factoryFunc);
    }
}

internal record HandlerMap(Type RequestType, Type HandlerType);