using CS.Mediator.Contract;

namespace CS.Mediator.Setup;

internal class MediatorConfiguration
{
    private readonly HashSet<HandlerMap> _handlers;
    private readonly List<Func<IServiceProvider, IPipelineFilter>> _pipelineFilters;

    public MediatorConfiguration(HashSet<HandlerMap> handlers, List<Func<IServiceProvider, IPipelineFilter>> pipelineFilters)
    {
        _handlers = handlers;
        _pipelineFilters = pipelineFilters;
    }

    public IList<Func<IServiceProvider, IPipelineFilter>> GetFilters()
    {
        return _pipelineFilters;
    }

    public Type GetHandler<TRequest>()
    {
        var mapping = _handlers.FirstOrDefault(o => o.RequestType == typeof(TRequest));
        if (mapping is not null)
        {
            return mapping.HandlerType;
        }

        throw new InvalidOperationException($"No handler registered for type {typeof(TRequest)}");
    }
}