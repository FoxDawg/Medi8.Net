using System;
using System.Linq;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Pipeline;

internal sealed class AsyncValidationMiddleware : IPreProcessor
{
    public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
    {
        var service = context.GetService(typeof(IValidateRequest<TRequest>));
        if (service is IValidateRequest<TRequest> validator)
        {
            var validationResults = await validator.ValidateAsync(context).ConfigureAwait(false);
            if (validationResults.Any())
            {
                context.WriteTo(validationResults);
                context.WriteTo(StatusCodes.ValidationFailed);
            }
            else
            {
                await next(context).ConfigureAwait(false);
            }
        }

        await next(context).ConfigureAwait(false);
    }
}