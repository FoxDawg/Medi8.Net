using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Tests.Commands;

public record CreateEntityCommand(string Name) : ICommand<CreateEntityCommand.EntityCreated>
{
    public record EntityCreated(long Id);

    public class CreateEntityCommandValidator : IValidateRequest<CreateEntityCommand>
    {
        public Task<ProcessingResults> ValidateAsync(ProcessingContext<CreateEntityCommand> context)
        {
            if (context.Request.Name.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new ProcessingResults(new List<ProcessingResult> { new(nameof(Name), "Cannot have name containing 'invalid'") }));
            }

            return Task.FromResult(ProcessingResults.Empty);
        }
    }

    public class CreateEntityCommandHandler : ICommandHandler<CreateEntityCommand, EntityCreated>
    {
        public Task<EntityCreated> HandleAsync(ProcessingContext<CreateEntityCommand, EntityCreated> context)
        {
            return Task.FromResult(new EntityCreated(25));
        }
    }
}