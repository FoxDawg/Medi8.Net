using System;
using System.Threading.Tasks;
using Mediator.Contract;
using Mediator.Handler;

namespace Mediator.Tests.Commands;

public record ThrowExceptionCommand : ICommand<EmptyResult>
{
    public class ThrowExceptionCommandHandler : CommandHandlerBase<ThrowExceptionCommand, EmptyResult>
    {
        public override Task<EmptyResult> HandleAsync(ProcessingContext<ThrowExceptionCommand, EmptyResult> context)
        {
            throw new ArithmeticException("This was intended!");
        }
    }
}