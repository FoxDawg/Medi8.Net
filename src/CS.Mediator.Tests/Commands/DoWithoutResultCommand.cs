using CS.Mediator.Contract;
using CS.Mediator.Handler;

namespace CS.Mediator.Tests.Commands;

public record DoWithoutResultCommand(string Parameter) : ICommand<EmptyResult>
{
    public class DoWithoutResultCommandHandler : CommandHandlerBase<DoWithoutResultCommand, EmptyResult>
    {
        public override async Task<ValidationResults> ValidateAsync(DoWithoutResultCommand command, CancellationToken token)
        {
            await Task.CompletedTask;
            if (command.Parameter.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            {
                return new ValidationResults(new List<ValidationResult> { new(nameof(Parameter), "Invalid operation") });
            }

            return ValidationResults.Empty;
        }

        public override Task<EmptyResult> HandleAsync(ProcessingContext<DoWithoutResultCommand, EmptyResult> context)
        {
            return Task.FromResult(EmptyResult.Create);
        }
    }
}