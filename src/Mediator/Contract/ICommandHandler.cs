using System.Threading.Tasks;

namespace Mediator.Contract;

public interface ICommandHandler <TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : class?
{
    Task ValidateAsync(ProcessingContext<TCommand, TResponse> context);
    Task<TResponse> HandleAsync(ProcessingContext<TCommand, TResponse> context);
}

public interface IQueryHandler <TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : class?
{
    Task ValidateAsync(ProcessingContext<TQuery, TResponse> context);
    Task<TResponse> HandleAsync(ProcessingContext<TQuery, TResponse> context);
}