using System;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Tests.Commands;

public record ThrowExceptionCommand : ICommand
{
    public class ThrowExceptionCommandHandler : ICommandHandler<ThrowExceptionCommand>
    {
        public Task HandleAsync(IProcessingContext<ThrowExceptionCommand> context)
        {
            throw new ArithmeticException("This was intended!");
        }
    }
}