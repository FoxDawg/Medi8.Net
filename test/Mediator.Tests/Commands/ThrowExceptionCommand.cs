using System;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Tests.Commands;

public record ThrowExceptionCommand : ICommand<NoResult>
{
    public class ThrowExceptionCommandHandler : ICommandHandler<ThrowExceptionCommand, NoResult>
    {
        public Task<NoResult> HandleAsync(ProcessingContext<ThrowExceptionCommand, NoResult> context)
        {
            throw new ArithmeticException("This was intended!");
        }
    }
}