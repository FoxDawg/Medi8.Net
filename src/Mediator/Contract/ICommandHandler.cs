using System.Threading.Tasks;

namespace Mediator.Contract;

public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : class?
{
    Task<TResponse> HandleAsync(IProcessingContext<TCommand> context);
}

public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : class?
{
    Task<TResponse> HandleAsync(IProcessingContext<TQuery> context);
}