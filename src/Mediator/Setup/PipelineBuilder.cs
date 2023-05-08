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

    internal Next<TRequest> BuildPreProcessorPipeline<TRequest>()
        where TRequest : IRequest
    {
        var filterFactories = this.configuration.Preprocessors;
        return this.BuildPipeline<TRequest>(filterFactories);
    }

    internal Next<TRequest> BuildPostProcessorPipeline<TRequest>()
        where TRequest : IRequest
    {
        var filterFactories = this.configuration.Postprocessors;
        return this.BuildPipeline<TRequest>(filterFactories);
    }

    private Next<TRequest> BuildPipeline<TRequest>(IList<IProcessor> filters)
        where TRequest : IRequest
    {
        if (!filters.Any())
        {
            return _ => Task.CompletedTask;
        }

        var pipeline = new Next<TRequest>[filters.Count + 1];
        pipeline[filters.Count] = BuildLastInvocation<TRequest>();
        for (var i = filters.Count - 1; i >= 0; i--)
        {
            var filter = filters[i];
            var next = pipeline[i + 1];
            pipeline[i] = BuildInvocation(filter, next);
        }

        Next<TRequest> begin = ctx => pipeline[0].Invoke(ctx);
        return begin;
    }

    private static Next<TRequest> BuildInvocation<TRequest>(IProcessor filter, Next<TRequest> next)
        where TRequest : IRequest
    {
        return ctx =>
        {
            if (ctx.Token.IsCancellationRequested)
            {
                ctx.WriteTo(Status.CancellationRequested);
                ctx.WriteTo(new Error("Pipeline", "Cancellation was requested during pipeline execution."));
                return Task.CompletedTask;
            }

            return filter.InvokeAsync(ctx, next);
        };
    }

    private static Next<TRequest> BuildLastInvocation<TRequest>()
        where TRequest : IRequest
    {
        return ctx =>
        {
            if (ctx.Token.IsCancellationRequested)
            {
                ctx.WriteTo(Status.CancellationRequested);
                ctx.WriteTo(new Error("Pipeline", "Cancellation was requested during pipeline execution."));
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        };
    }
}