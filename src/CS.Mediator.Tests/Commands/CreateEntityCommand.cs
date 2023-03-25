using CS.Mediator.Contract;
using CS.Mediator.Handler;

namespace CS.Mediator.Tests.Commands;

public record CreateEntityCommand(string Name) : ICommand<CreateEntityCommand.EntityCreated>
{
    public record EntityCreated(long Id);

    public class CreateEntityCommandHandler : CommandHandlerBase<CreateEntityCommand, EntityCreated>
    {
        public override async Task<ValidationResults> ValidateAsync(CreateEntityCommand command, CancellationToken token)
        {
            await Task.CompletedTask;
            if (command.Name.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            {
                return new ValidationResults(new List<ValidationResult> { new(nameof(Name), "Cannot have name containing 'invalid'") });
            }

            return ValidationResults.Empty;
        }

        public override Task<EntityCreated> HandleAsync(ProcessingContext<CreateEntityCommand, EntityCreated> context)
        {
            return Task.FromResult(new EntityCreated(25));
        }
    }
}