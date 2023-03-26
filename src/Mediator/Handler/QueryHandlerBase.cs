using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Handler;

public abstract class QueryHandlerBase<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : class?
{
    public async Task ValidateAsync(ProcessingContext<TQuery, TResponse> context)
    {
        var validationResults = await this.ValidateAsync(context.Request, context.Token).ConfigureAwait(false);

        if (validationResults.Any())
        {
            context.WriteTo(validationResults);
            context.WriteTo(StatusCode.BadRequest);
        }
    }

    public abstract Task<TResponse> HandleAsync(ProcessingContext<TQuery, TResponse> context);

    public virtual Task<ProcessingResults> ValidateAsync(TQuery query, CancellationToken token)
    {
        return Task.FromResult(new ProcessingResults());
    }
}