using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Tests.Commands;

public record DoWithoutResultCommand(string Parameter) : ICommand<NoResult>
{
    public class DoWithoutCommandValidator : IValidateRequest<DoWithoutResultCommand>
    {
        public Task<Errors> ValidateAsync(ProcessingContext<DoWithoutResultCommand> context)
        {
            if (context.Request.Parameter.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new Errors(new List<Error> { new(nameof(Parameter), "Invalid operation") }));
            }

            return Task.FromResult(Errors.Empty);
        }
    }

    public class DoWithoutResultCommandHandler : ICommandHandler<DoWithoutResultCommand, NoResult>
    {
        public Task<NoResult> HandleAsync(ProcessingContext<DoWithoutResultCommand, NoResult> context)
        {
            return Task.FromResult(NoResult.Create());
        }
    }
}