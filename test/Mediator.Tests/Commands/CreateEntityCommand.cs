using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Contract;
using Mediator.Handler;

namespace Mediator.Tests.Commands;

public record CreateEntityCommand(string Name) : ICommand<CreateEntityCommand.EntityCreated>
{
    public record EntityCreated(long Id);

    public class CreateEntityCommandHandler : CommandHandlerBase<CreateEntityCommand, EntityCreated>
    {
        public override Task<ProcessingResults> ValidateAsync(CreateEntityCommand command, CancellationToken token)
        {
            if (command.Name.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new ProcessingResults(new List<ProcessingResult> { new (nameof(CreateEntityCommand.Name), "Cannot have name containing 'invalid'") }));
            }

            return Task.FromResult(ProcessingResults.Empty);
        }

        public override Task<EntityCreated> HandleAsync(ProcessingContext<CreateEntityCommand, EntityCreated> context)
        {
            return Task.FromResult(new EntityCreated(25));
        }
    }
}