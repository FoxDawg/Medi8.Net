using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Tests.Commands;

public record DoWithoutResultCommand(string Parameter) : ICommand<EmptyResult>
{
    public class DoWithoutCommandValidator : IValidateRequest<DoWithoutResultCommand>
    {
        public Task<ProcessingResults> ValidateAsync(ProcessingContext<DoWithoutResultCommand> context)
        {
            if (context.Request.Parameter.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new ProcessingResults(new List<ProcessingResult> { new(nameof(Parameter), "Invalid operation") }));
            }

            return Task.FromResult(ProcessingResults.Empty);
        }
    }

    public class DoWithoutResultCommandHandler : ICommandHandler<DoWithoutResultCommand, EmptyResult>
    {
        public Task<EmptyResult> HandleAsync(ProcessingContext<DoWithoutResultCommand, EmptyResult> context)
        {
            return Task.FromResult(EmptyResult.Create);
        }
    }
}