using CS.Mediator.Contract;

namespace CS.Mediator.Handler;

public abstract class QueryHandlerBase<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse> 
    where TResponse : class?
{
    public async Task ValidateAsync(ProcessingContext<TQuery, TResponse> context)
    {
        var validationResults = await ValidateAsync(context.Request, context.Token);
        
        if (validationResults.Any())
        {
            context.WriteTo(validationResults);
            context.WriteTo(StatusCode.BadRequest);
        }
    }

    public abstract Task<TResponse> HandleAsync(ProcessingContext<TQuery, TResponse> context);

    public virtual async Task<ValidationResults> ValidateAsync(TQuery query, CancellationToken token)
    {
        await Task.CompletedTask;
        return new ValidationResults();
    }
}