using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mediator.Contract;
using Mediator.Pipeline;

namespace Mediator.Setup;

internal class PipelineBuilder
{
    private readonly MediatorConfiguration configuration;

    public PipelineBuilder(MediatorConfiguration configuration)
    {
        this.configuration = configuration;
    }

    internal Next<TRequest> BuildPreProcessorPipeline <TRequest>()
    {
        var filterFactories = this.configuration.Preprocessors;
        return this.BuildPipeline<TRequest>(filterFactories);
    }

    internal Next<TRequest> BuildPostProcessorPipeline <TRequest>()
    {
        var filterFactories = this.configuration.Postprocessors;
        return this.BuildPipeline<TRequest>(filterFactories);
    }

    private Next<TRequest> BuildPipeline <TRequest>(IList<IProcessor> filters)
    {
        if (!filters.Any())
        {
            return _ => Task.CompletedTask;
        }

        var pipeline = new Next<TRequest>[filters.Count];
        for (var i = filters.Count - 1; i >= 0; i--)
        {
            var filter = filters[i];
            if (i == filters.Count - 1)
            {
                pipeline[i] = ctx => filter.InvokeAsync(ctx, _ => Task.CompletedTask);
            }
            else
            {
                var next = pipeline[i + 1];
                pipeline[i] = ctx => filter.InvokeAsync(ctx, next);
            }
        }

        Next<TRequest> begin = ctx => pipeline[0].Invoke(ctx);
        return begin;
    }
}