using System;
using System.Linq;
using System.Threading.Tasks;

namespace CS.Mediator.Setup;

internal class PipelineBuilder
{
    private readonly MediatorConfiguration configuration;
    private readonly IServiceProvider provider;

    public PipelineBuilder(MediatorConfiguration configuration, IServiceProvider provider)
    {
        this.configuration = configuration;
        this.provider = provider;
    }

    internal NextFilter Build()
    {
        var filterFactories = this.configuration.GetFilters();
        if (!filterFactories.Any())
        {
            return _ => Task.CompletedTask;
        }

        var pipeline = new NextFilter[filterFactories.Count];
        for (var i = filterFactories.Count - 1; i >= 0; i--)
        {
            var filter = filterFactories[i](this.provider);
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