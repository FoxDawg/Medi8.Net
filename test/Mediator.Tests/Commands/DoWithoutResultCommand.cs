using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Tests.Commands;

public record DoWithoutResultCommand(string Parameter) : ICommand
{
    public class DoWithoutCommandValidator : IValidator<DoWithoutResultCommand>
    {
        public Task<Errors> ValidateAsync(IProcessingContext<DoWithoutResultCommand> context)
        {
            if (context.Request.Parameter.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new Errors(new List<Error> { new(nameof(Parameter), "Invalid operation") }));
            }

            return Task.FromResult(Errors.Empty);
        }
    }

    public class DoWithoutResultCommandHandler : ICommandHandler<DoWithoutResultCommand>
    {
        public Task HandleAsync(IProcessingContext<DoWithoutResultCommand> context)
        {
            return Task.CompletedTask;
        }
    }
}