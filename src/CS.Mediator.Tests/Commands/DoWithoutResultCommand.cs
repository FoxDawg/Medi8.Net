using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CS.Mediator.Contract;
using CS.Mediator.Handler;

namespace CS.Mediator.Tests.Commands;

public record DoWithoutResultCommand(string Parameter) : ICommand<EmptyResult>
{
    public class DoWithoutResultCommandHandler : CommandHandlerBase<DoWithoutResultCommand, EmptyResult>
    {
        public override Task<ValidationResults> ValidateAsync(DoWithoutResultCommand command, CancellationToken token)
        {
            if (command.Parameter.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new ValidationResults(new List<ValidationResult> { new (nameof(DoWithoutResultCommand.Parameter), "Invalid operation") }));
            }

            return Task.FromResult(ValidationResults.Empty);
        }

        public override Task<EmptyResult> HandleAsync(ProcessingContext<DoWithoutResultCommand, EmptyResult> context)
        {
            return Task.FromResult(EmptyResult.Create);
        }
    }
}