using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Tests.Commands;

public record CreateEntityCommand(string Name) : ICommand<CreateEntityCommand.EntityCreated>
{
    // ReSharper disable once NotAccessedPositionalProperty.Global
    public record EntityCreated(long Id);

    public class CreateEntityCommandValidator : IValidator<CreateEntityCommand>
    {
        public Task<Errors> ValidateAsync(IProcessingContext<CreateEntityCommand> context)
        {
            if (context.Request.Name.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new Errors(new List<Error> { new(nameof(Name), "Cannot have name containing 'invalid'") }));
            }

            return Task.FromResult(Errors.Empty);
        }
    }

    public class CreateEntityCommandHandler : ICommandHandler<CreateEntityCommand, EntityCreated>
    {
        public Task<EntityCreated> HandleAsync(IProcessingContext<CreateEntityCommand> context)
        {
            return Task.FromResult(new EntityCreated(25));
        }
    }
}