using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CS.Mediator.Contract;

namespace CS.Mediator.Handler;

public abstract class CommandHandlerBase<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : class?
{
    public async Task ValidateAsync(ProcessingContext<TCommand, TResponse> context)
    {
        var validationResults = await this.ValidateAsync(context.Request, context.Token).ConfigureAwait(false);

        if (validationResults.Any())
        {
            context.WriteTo(validationResults);
            context.WriteTo(StatusCode.BadRequest);
        }
    }

    public abstract Task<TResponse> HandleAsync(ProcessingContext<TCommand, TResponse> context);

    public virtual Task<ValidationResults> ValidateAsync(TCommand command, CancellationToken token)
    {
        return Task.FromResult(new ValidationResults());
    }
}