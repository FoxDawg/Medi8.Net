using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mediator.Contract;
using Mediator.Pipeline;

namespace Mediator.Setup;

internal class PipelineBuilder
{
    private readonly MediatorConfiguration configuration;
    private readonly IServiceProvider provider;

    public PipelineBuilder(MediatorConfiguration configuration, IServiceProvider provider)
    {
        this.configuration = configuration;
        this.provider = provider;
    }

    internal NextFilter BuildPreProcessorPipeline()
    {
        var filterFactories = this.configuration.Preprocessors;
        return this.BuildPipeline(filterFactories);
    }

    internal NextFilter BuildPostProcessorPipeline()
    {
        var filterFactories = this.configuration.Postprocessors;
        return this.BuildPipeline(filterFactories);
    }

    private NextFilter BuildPipeline(IList<Func<IServiceProvider, IProcessor>> filterFactories)
    {
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