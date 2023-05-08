using System.Linq;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Pipeline;

internal sealed class AsyncValidationMiddleware : IPreProcessor
{
    public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
        where TRequest : IRequest
    {
        var service = context.GetService(typeof(IValidator<TRequest>));
        if (service is IValidator<TRequest> validator)
        {
            var validationResults = await validator.ValidateAsync(context).ConfigureAwait(false);
            if (validationResults.Any())
            {
                context.WriteTo(validationResults);
                context.WriteTo(Status.ValidationFailed);
                return;
            }
        }

        await next(context).ConfigureAwait(false);
    }
}