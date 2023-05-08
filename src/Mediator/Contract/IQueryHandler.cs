using System.Threading.Tasks;

namespace Mediator.Contract;

public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : class?
{
    Task<TResponse> HandleAsync(IProcessingContext<TQuery> context);
}