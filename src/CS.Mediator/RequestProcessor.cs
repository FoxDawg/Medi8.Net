using CS.Mediator.Contract;
using CS.Mediator.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace CS.Mediator;

internal sealed class RequestProcessor : IMediator
{
    private readonly MediatorConfiguration _configuration;
    private readonly IServiceProvider _provider;
    private readonly PipelineBuilder _pipelineBuilder;

    public RequestProcessor(MediatorConfiguration configuration, IServiceProvider provider, PipelineBuilder pipelineBuilder)
    {
        _configuration = configuration;
        _provider = provider;
        _pipelineBuilder = pipelineBuilder;
    }

    private async Task ExecutePipelineAsync(ProcessingContext context)
    {
        var pipelineStart = _pipelineBuilder.Build();
        await pipelineStart.Invoke(context);
    }

    public async Task<RequestResult<EmptyResult>> HandleCommandAsync<TCommand>(TCommand command, CancellationToken token)
        where TCommand : ICommand<EmptyResult>
    {
        return await HandleCommandAsync<TCommand, EmptyResult>(command, token);
    }

    public async Task<RequestResult<TResult>> HandleCommandAsync<TCommand, TResult>(TCommand command, CancellationToken token)
        where TCommand : ICommand<TResult>
        where TResult : class?
    {
        var context = new ProcessingContext<TCommand, TResult>(command, token);

        await ExecutePipelineAsync(context);

        if (context.StatusCode != StatusCode.Ok)
        {
            context.WriteTo(new[] { new ValidationResult("Pipeline", $"Filter pipeline failed returned {context.StatusCode}") });
            return context.ToRequestResult();
        }

        var handler = _provider.GetRequiredService(_configuration.GetHandler<TCommand>());
        if (handler is ICommandHandler<TCommand, TResult> commandHandler)
        {
            return await HandleCommandInternalAsync(commandHandler, context);
        }

        throw new InvalidCastException($"Registered handler is not of type {typeof(ICommandHandler<TCommand, TResult>)}");
    }

    public async Task<RequestResult<TResult>> HandleQueryAsync<TQuery, TResult>(TQuery query, CancellationToken token)
        where TQuery : IQuery<TResult>
        where TResult : class?
    {
        var context = new ProcessingContext<TQuery, TResult>(query, token);

        await ExecutePipelineAsync(context);

        if (context.StatusCode != StatusCode.Ok)
        {
            context.WriteTo(new[] { new ValidationResult("Pipeline", $"Filter pipeline failed returned {context.StatusCode}") });
            return context.ToRequestResult();
        }
        
        var handler = _provider.GetRequiredService(_configuration.GetHandler<TQuery>());
        if (handler is IQueryHandler<TQuery, TResult> queryHandler)
        {
            return await HandleQueryInternalAsync(queryHandler, context);
        }

        throw new InvalidCastException($"Registered handler is not of type {typeof(IQueryHandler<TQuery, TResult>)}");
    }

    private async Task<RequestResult<TResult>> HandleCommandInternalAsync<TCommand, TResult>(
        ICommandHandler<TCommand, TResult> handler,
        ProcessingContext<TCommand, TResult> context)
        where TCommand : ICommand<TResult>
        where TResult : class?
    {
        await handler.ValidateAsync(context);

        if (!context.IsValid)
        {
            return context.ToRequestResult();
        }

        var result = await handler.HandleAsync(context);
        context.WriteTo(result);
        
        return context.ToRequestResult();
    }

    private async Task<RequestResult<TResult>> HandleQueryInternalAsync<TQuery, TResult>(
        IQueryHandler<TQuery, TResult> handler,
        ProcessingContext<TQuery, TResult> context)
        where TQuery : IQuery<TResult>
        where TResult : class?
    {
        await handler.ValidateAsync(context);

        if (!context.IsValid)
        {
            return context.ToRequestResult();
        }

        var result = await handler.HandleAsync(context);
        context.WriteTo(result);
        return context.ToRequestResult();
    }
}