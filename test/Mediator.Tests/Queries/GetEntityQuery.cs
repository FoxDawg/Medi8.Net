using System;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Tests.Queries;

public record GetEntityQuery(long Id) : IQuery<GetEntityQuery.Entity>
{
    internal const int InvalidId = -1;
    internal const int ValidId = 15;
    internal const int NonExistingId = 1;
    internal const int ExceptionId = 0;

    public record Entity(long Id);

    public class GetEntityQueryValidator : IValidateRequest<GetEntityQuery>
    {
        public Task<Errors> ValidateAsync(ProcessingContext<GetEntityQuery> context)
        {
            if (long.IsNegative(context.Request.Id))
            {
                return Task.FromResult(new Errors(new[] { new Error(nameof(Id), "Entity ID must be greater 0") }));
            }

            return Task.FromResult(Errors.Empty);
        }
    }

    public class GetEntityQueryHandler : IQueryHandler<GetEntityQuery, Entity?>
    {
        public async Task<Entity?> HandleAsync(ProcessingContext<GetEntityQuery, Entity?> context)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            return context.Request.Id switch
            {
                ExceptionId => throw new ArithmeticException(),
                NonExistingId => null,
                _ => new Entity(context.Request.Id)
            };
        }
    }
}