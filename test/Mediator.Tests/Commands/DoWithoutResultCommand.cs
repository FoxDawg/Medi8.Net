using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Contract;
using Mediator.Handler;

namespace Mediator.Tests.Commands;

public record DoWithoutResultCommand(string Parameter) : ICommand<EmptyResult>
{
    public class DoWithoutResultCommandHandler : CommandHandlerBase<DoWithoutResultCommand, EmptyResult>
    {
        public override Task<ProcessingResults> ValidateAsync(DoWithoutResultCommand command, CancellationToken token)
        {
            if (command.Parameter.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new ProcessingResults(new List<ProcessingResult> { new (nameof(DoWithoutResultCommand.Parameter), "Invalid operation") }));
            }

            return Task.FromResult(ProcessingResults.Empty);
        }

        public override Task<EmptyResult> HandleAsync(ProcessingContext<DoWithoutResultCommand, EmptyResult> context)
        {
            return Task.FromResult(EmptyResult.Create);
        }
    }
}