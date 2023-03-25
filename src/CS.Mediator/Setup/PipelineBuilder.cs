namespace CS.Mediator.Setup;

internal class PipelineBuilder
{
    private readonly MediatorConfiguration _configuration;
    private readonly IServiceProvider _provider;

    public PipelineBuilder(MediatorConfiguration configuration, IServiceProvider provider)
    {
        _configuration = configuration;
        _provider = provider;
    }

    internal NextFilter Build()
    {
        var filterFactories = _configuration.GetFilters();
        if (!filterFactories.Any())
        {
            return _ => Task.CompletedTask;
        }

        var pipeline = new NextFilter[filterFactories.Count];
        for (var i = filterFactories.Count - 1; i >= 0; i--)
        {
            var filter = filterFactories[i](_provider);
            if (i == filterFactories.Count - 1)
            {
                pipeline[i] = ctx => filter.InvokeAsync(ctx, _ => Task.CompletedTask);
            }
            else
            {
                var next = pipeline[i + 1];
                pipeline[i] = ctx => filter.InvokeAsync(ctx, next);
            }
        }

        NextFilter begin = ctx => pipeline[0].Invoke(ctx);
        return begin;
    }
}