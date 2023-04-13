using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Pipeline;

public delegate Task Next<TRequest>(ProcessingContext<TRequest> context)
    where TRequest : IRequest;