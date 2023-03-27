using System;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Tests.Commands;

public record ThrowExceptionCommand : ICommand<EmptyResult>
{
    public class ThrowExceptionCommandHandler : ICommandHandler<ThrowExceptionCommand, EmptyResult>
    {
        public Task<EmptyResult> HandleAsync(ProcessingContext<ThrowExceptionCommand, EmptyResult> context)
        {
            throw new ArithmeticException("This was intended!");
        }
    }
}