using System;
using System.Threading;
using System.Threading.Tasks;
using CS.Mediator.Contract;
using CS.Mediator.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace CS.Mediator;

internal sealed class RequestProcessor : IMediator
{
    private readonly MediatorConfiguration configuration;
    private readonly PipelineBuilder pipelineBuilder;
    private readonly IServiceProvider provider;

    public RequestProcessor(MediatorConfiguration configuration, IServiceProvider provider, PipelineBuilder pipelineBuilder)
    {
        this.configuration = configuration;
        this.provider = provider;
        this.pipelineBuilder = pipelineBuilder;
    }

    public async Task<RequestResult<EmptyResult>> HandleCommandAsync <TCommand>(TCommand command, CancellationToken token)
        where TCommand : ICommand<EmptyResult>
    {
        return await this.HandleCommandAsync<TCommand, EmptyResult>(command, token).ConfigureAwait(false);
    }

    public async Task<RequestResult<TResult>> HandleCommandAsync <TCommand, TResult>(TCommand command, CancellationToken token)
        where TCommand : ICommand<TResult>
        where TResult : class?
    {
        var context = new ProcessingContext<TCommand, TResult>(command, token);

        await this.ExecutePipelineAsync(context).ConfigureAwait(false);

        if (context.StatusCode != StatusCode.Ok)
        {
            context.WriteTo(new[] {new ValidationResult("Pipeline", $"Filter pipeline failed returned {context.StatusCode}") });
            return context.ToRequestResult();
        }

        var handler = this.provider.GetRequiredService(this.configuration.GetHandler<TCommand>());
        if (handler is ICommandHandler<TCommand, TResult> commandHandler)
        {
            return await HandleCommandInternalAsync(commandHandler, context).ConfigureAwait(false);
        }

        throw new InvalidCastException($"Registered handler is not of type {typeof(ICommandHandler<TCommand, TResult>)}");
    }

    public async Task<RequestResult<TResult>> HandleQueryAsync <TQuery, TResult>(TQuery query, CancellationToken token)
        where TQuery : IQuery<TResult>
        where TResult : class?
    {
        var context = new ProcessingContext<TQuery, TResult>(query, token);

        await this.ExecutePipelineAsync(context).ConfigureAwait(false);

        if (context.StatusCode != StatusCode.Ok)
        {
            context.WriteTo(new[] {new ValidationResult("Pipeline", $"Filter pipeline failed returned {context.StatusCode}") });
            return context.ToRequestResult();
        }

        var handler = this.provider.GetRequiredService(this.configuration.GetHandler<TQuery>());
        if (handler is IQueryHandler<TQuery, TResult> queryHandler)
        {
            return await HandleQueryInternalAsync(queryHandler, context).ConfigureAwait(false);
        }

        throw new InvalidCastException($"Registered handler is not of type {typeof(IQueryHandler<TQuery, TResult>)}");
    }

    private async Task ExecutePipelineAsync(ProcessingContext context)
    {
        var pipelineStart = this.pipelineBuilder.Build();
        await pipelineStart.Invoke(context).ConfigureAwait(false);
    }

    private static async Task<RequestResult<TResult>> HandleCommandInternalAsync <TCommand, TResult>(
        ICommandHandler<TCommand, TResult> handler,
        ProcessingContext<TCommand, TResult> context)
        where TCommand : ICommand<TResult>
        where TResult : class?
    {
        await handler.ValidateAsync(context).ConfigureAwait(false);

        if (!context.IsValid)
        {
            return context.ToRequestResult();
        }

        var result = await handler.HandleAsync(context).ConfigureAwait(false);
        context.WriteTo(result);

        return context.ToRequestResult();
    }

    private static async Task<RequestResult<TResult>> HandleQueryInternalAsync <TQuery, TResult>(
        IQueryHandler<TQuery, TResult> handler,
        ProcessingContext<TQuery, TResult> context)
        where TQuery : IQuery<TResult>
        where TResult : class?
    {
        await handler.ValidateAsync(context).ConfigureAwait(false);

        if (!context.IsValid)
        {
            return context.ToRequestResult();
        }

        var result = await handler.HandleAsync(context).ConfigureAwait(false);
        context.WriteTo(result);
        return context.ToRequestResult();
    }
}