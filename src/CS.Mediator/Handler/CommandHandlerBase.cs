using CS.Mediator.Contract;

namespace CS.Mediator.Handler;

public abstract class CommandHandlerBase<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> 
    where TResponse : class?
{
    public async Task ValidateAsync(ProcessingContext<TCommand, TResponse> context)
    {
        var validationResults = await ValidateAsync(context.Request, context.Token);
                
        if (validationResults.Any())
        {
            context.WriteTo(validationResults);
            context.WriteTo(StatusCode.BadRequest);
        }
    }

    public abstract Task<TResponse> HandleAsync(ProcessingContext<TCommand, TResponse> context);

    public virtual async Task<ValidationResults> ValidateAsync(TCommand command, CancellationToken token)
    {
        await Task.CompletedTask;
        return new ValidationResults();
    }
}