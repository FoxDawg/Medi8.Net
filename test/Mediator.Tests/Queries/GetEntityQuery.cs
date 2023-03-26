using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Contract;
using Mediator.Handler;

namespace Mediator.Tests.Queries;

public record GetEntityQuery(long Id) : IQuery<GetEntityQuery.Entity>
{
    internal const int InvalidId = -1;
    internal const int ValidId = 15;
    internal const int NonExistingId = 1;
    internal const int ExceptionId = 0;

    public record Entity(long Id);

    public class GetEntityQueryHandler : QueryHandlerBase<GetEntityQuery, Entity?>
    {
        public override Task<ProcessingResults> ValidateAsync(GetEntityQuery query, CancellationToken token)
        {
            if (long.IsNegative(query.Id))
            {
                return Task.FromResult(new ProcessingResults(new[] { new ProcessingResult(nameof(GetEntityQuery.Id), "Entity ID must be greater 0") }));
            }

            return Task.FromResult(ProcessingResults.Empty);
        }

        public override async Task<Entity?> HandleAsync(ProcessingContext<GetEntityQuery, Entity?> context)
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