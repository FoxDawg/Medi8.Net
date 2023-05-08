using System.Threading.Tasks;

namespace Mediator.Contract;

public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : class?
{
    Task<TResponse> HandleAsync(IProcessingContext<TCommand> context);
}

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task HandleAsync(IProcessingContext<TCommand> context);
}