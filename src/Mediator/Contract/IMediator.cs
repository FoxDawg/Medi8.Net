using System.Threading;
using System.Threading.Tasks;

namespace Mediator.Contract;

public interface IMediator
{
    Task<RequestResult<TResult>> HandleQueryAsync<TQuery, TResult>(TQuery query, CancellationToken token)
        where TQuery : IQuery<TResult>
        where TResult : class?;

    Task<RequestResult<TResult>> HandleCommandAsync<TCommand, TResult>(TCommand command, CancellationToken token)
        where TCommand : ICommand<TResult>
        where TResult : class?;

    Task<RequestResult> HandleCommandAsync<TCommand>(TCommand command, CancellationToken token)
        where TCommand : ICommand;
}